using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, ServiceResult<CreateInvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const decimal DefaultTaxRate = 0.18m; // %18 KDV
    private const int DefaultDueDateDays = 30;

    public CreateInvoiceHandler(
        IInvoiceRepository invoiceRepository,
        IRentalRepository rentalRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CreateInvoiceResponse>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Check if rental exists
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental == null)
        {
            return ServiceResult<CreateInvoiceResponse>.Fail($"Rental with ID {request.RentalId} not found.", HttpStatusCode.NotFound);
        }

        // Check if rental is completed
        if (rental.Status != RentalStatus.Completed)
        {
            return ServiceResult<CreateInvoiceResponse>.Fail($"Invoice can only be created for completed rentals. Current rental status: {rental.Status}.", HttpStatusCode.BadRequest);
        }

        // Check if rental already has an invoice
        var existingInvoice = await _invoiceRepository.GetByRentalIdAsync(request.RentalId);
        if (existingInvoice != null)
        {
            return ServiceResult<CreateInvoiceResponse>.Fail($"Rental with ID {request.RentalId} already has an invoice.", HttpStatusCode.Conflict);
        }

        // Generate invoice number
        var invoiceNumber = await GenerateInvoiceNumberAsync();

        // Calculate amounts from rental
        var subTotal = rental.TotalAmount;
        if (rental.LateFee.HasValue)
        {
            subTotal += rental.LateFee.Value;
        }
        if (rental.DamageFee.HasValue)
        {
            subTotal += rental.DamageFee.Value;
        }

        var taxRate = request.TaxRate ?? DefaultTaxRate;
        var taxAmount = subTotal * taxRate;
        var totalAmount = subTotal + taxAmount;

        // Set due date
        var invoiceDate = DateTime.UtcNow;
        var dueDate = request.DueDate ?? invoiceDate.AddDays(DefaultDueDateDays);

        // Get customer information from rental
        var customer = rental.Customer;
        var customerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : null;

        // Create invoice entity
        var invoice = new Domain.Entities.Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            TotalAmount = totalAmount,
            CustomerName = customerName,
            CustomerEmail = customer?.Email,
            Notes = request.Notes,
            RentalId = request.RentalId,
            IsPaid = false
        };

        var createdInvoice = await _invoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        // Reload invoice
        createdInvoice = await _invoiceRepository.GetByIdAsync(createdInvoice.Id);

        // Map to response
        var response = _mapper.Map<CreateInvoiceResponse>(createdInvoice);

        return ServiceResult<CreateInvoiceResponse>.Success(response, HttpStatusCode.Created);
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        // Format: INV-YYYYMMDD-XXXX (e.g., INV-20240115-0001)
        var today = DateTime.UtcNow;
        var datePrefix = today.ToString("yyyyMMdd");
        var prefix = $"INV-{datePrefix}";

        // Find the last invoice number for today
        var todayInvoices = await _invoiceRepository.GetByDateRangeAsync(today.Date, today.Date.AddDays(1));
        var todayCount = todayInvoices.Count();
        var sequenceNumber = (todayCount + 1).ToString("D4"); // 4 digit with leading zeros

        var invoiceNumber = $"{prefix}-{sequenceNumber}";

        // Double check if it exists (race condition için)
        if (await _invoiceRepository.IsInvoiceNumberExistsAsync(invoiceNumber))
        {
            // Retry with incremented number
            var retryCount = 0;
            do
            {
                retryCount++;
                sequenceNumber = (todayCount + 1 + retryCount).ToString("D4");
                invoiceNumber = $"{prefix}-{sequenceNumber}";
            } while (await _invoiceRepository.IsInvoiceNumberExistsAsync(invoiceNumber) && retryCount < 100);
        }

        return invoiceNumber;
    }
}

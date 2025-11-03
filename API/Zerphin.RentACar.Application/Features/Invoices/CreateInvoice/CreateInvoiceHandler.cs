using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;

namespace Zerphin.RentACar.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, ServiceResult<CreateInvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IInvoiceNumberGenerator _invoiceNumberGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const decimal DefaultTaxRate = 0.18m; // %18 KDV
    private const int DefaultDueDateDays = 30;

    public CreateInvoiceHandler(
        IInvoiceRepository invoiceRepository,
        IRentalRepository rentalRepository,
        IInvoiceNumberGenerator invoiceNumberGenerator,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _rentalRepository = rentalRepository;
        _invoiceNumberGenerator = invoiceNumberGenerator;
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

        // Check if rental already has an invoice (tekrar fatura oluşturulmasın)
        var existingInvoice = await _invoiceRepository.GetByRentalIdAsync(request.RentalId);
        if (existingInvoice != null)
        {
            return ServiceResult<CreateInvoiceResponse>.Fail($"Rental with ID {request.RentalId} already has an invoice.", HttpStatusCode.Conflict);
        }

        // Generate invoice number
        var invoiceNumber = await _invoiceNumberGenerator.GenerateInvoiceNumberAsync();

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
}

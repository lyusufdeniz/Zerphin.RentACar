using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Invoices.UpdateInvoice;

public class UpdateInvoiceHandler : IRequestHandler<UpdateInvoiceCommand, ServiceResult<UpdateInvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateInvoiceHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UpdateInvoiceResponse>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        if (invoice == null)
        {
            return ServiceResult<UpdateInvoiceResponse>.Fail($"Invoice with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Check if invoice number is being changed and if new one already exists
        if (invoice.InvoiceNumber != request.InvoiceNumber)
        {
            if (await _invoiceRepository.IsInvoiceNumberExistsAsync(request.InvoiceNumber))
            {
                return ServiceResult<UpdateInvoiceResponse>.Fail($"Invoice with invoice number '{request.InvoiceNumber}' already exists.", HttpStatusCode.Conflict);
            }
        }

        // Update invoice properties
        _mapper.Map(request, invoice);
        await _invoiceRepository.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        // Reload invoice
        invoice = await _invoiceRepository.GetByIdAsync(invoice.Id);

        var response = _mapper.Map<UpdateInvoiceResponse>(invoice);
        return ServiceResult<UpdateInvoiceResponse>.Success(response);
    }
}




using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Invoices.DeleteInvoice;

public class DeleteInvoiceHandler : IRequestHandler<DeleteInvoiceCommand, ServiceResult<DeleteInvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInvoiceHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<DeleteInvoiceResponse>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        if (invoice == null)
        {
            return ServiceResult<DeleteInvoiceResponse>.Fail($"Invoice with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Soft delete invoice
        await _invoiceRepository.DeleteAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteInvoiceResponse
        {
            Id = request.Id,
            Message = "Invoice deleted successfully."
        };

        return ServiceResult<DeleteInvoiceResponse>.Success(response);
    }
}



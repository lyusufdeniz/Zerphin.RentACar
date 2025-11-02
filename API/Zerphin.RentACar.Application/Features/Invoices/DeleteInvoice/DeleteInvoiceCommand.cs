using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Invoices.DeleteInvoice;

public class DeleteInvoiceCommand : IRequest<ServiceResult<DeleteInvoiceResponse>>
{
    public Guid Id { get; set; }
}



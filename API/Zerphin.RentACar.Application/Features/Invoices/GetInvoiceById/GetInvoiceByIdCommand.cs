using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Invoices.GetInvoiceById;

public class GetInvoiceByIdCommand : IRequest<ServiceResult<GetInvoiceByIdResponse>>
{
    public Guid Id { get; set; }
}


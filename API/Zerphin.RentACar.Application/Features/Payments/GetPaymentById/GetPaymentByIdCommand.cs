using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Payments.GetPaymentById;

public class GetPaymentByIdCommand : IRequest<ServiceResult<GetPaymentByIdResponse>>
{
    public Guid Id { get; set; }
}


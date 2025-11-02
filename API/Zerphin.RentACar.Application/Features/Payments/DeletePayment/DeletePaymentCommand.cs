using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Payments.DeletePayment;

public class DeletePaymentCommand : IRequest<ServiceResult<DeletePaymentResponse>>
{
    public Guid Id { get; set; }
}



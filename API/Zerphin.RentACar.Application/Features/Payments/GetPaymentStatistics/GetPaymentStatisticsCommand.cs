using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Payments.GetPaymentStatistics;

public class GetPaymentStatisticsCommand : IRequest<ServiceResult<GetPaymentStatisticsResponse>>
{
}


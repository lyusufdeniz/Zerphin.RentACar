using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Insurances.GetInsuranceStatistics;

public class GetInsuranceStatisticsCommand : IRequest<ServiceResult<GetInsuranceStatisticsResponse>>
{
}


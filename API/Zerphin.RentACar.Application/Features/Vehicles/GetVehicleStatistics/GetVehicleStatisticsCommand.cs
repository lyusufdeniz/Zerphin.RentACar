using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Vehicles.GetVehicleStatistics;

public class GetVehicleStatisticsCommand : IRequest<ServiceResult<GetVehicleStatisticsResponse>>
{
}


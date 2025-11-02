using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Rentals.GetRentalStatistics;

public class GetRentalStatisticsCommand : IRequest<ServiceResult<GetRentalStatisticsResponse>>
{
}


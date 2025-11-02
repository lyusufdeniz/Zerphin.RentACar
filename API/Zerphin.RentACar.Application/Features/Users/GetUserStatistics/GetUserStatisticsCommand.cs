using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.GetUserStatistics;

public class GetUserStatisticsCommand : IRequest<ServiceResult<GetUserStatisticsResponse>>
{
}


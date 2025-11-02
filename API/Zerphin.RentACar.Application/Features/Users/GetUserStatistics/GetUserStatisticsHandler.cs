using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Users.GetUserStatistics;

public class GetUserStatisticsHandler : IRequestHandler<GetUserStatisticsCommand, ServiceResult<GetUserStatisticsResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserStatisticsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<GetUserStatisticsResponse>> Handle(GetUserStatisticsCommand request, CancellationToken cancellationToken)
    {
        var response = new GetUserStatisticsResponse();

        response.TotalUsers = await _userRepository.CountAsync();
        response.ActiveUsers = await _userRepository.CountAsync(u => u.IsActive);
        response.InactiveUsers = await _userRepository.CountAsync(u => !u.IsActive);
        response.NewUsersLast30Days = await _userRepository.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));

        // Role istatistikleri
        var roles = Enum.GetValues<UserRole>();
        var roleStats = new List<UserRoleStatistics>();
        
        foreach (var role in roles)
        {
            var count = await _userRepository.CountAsync(u => u.Role == role);
            roleStats.Add(new UserRoleStatistics
            {
                Role = role,
                RoleName = role.ToString(),
                Count = count
            });
        }
        
        response.RoleStatistics = roleStats;

        return ServiceResult<GetUserStatisticsResponse>.Success(response);
    }
}


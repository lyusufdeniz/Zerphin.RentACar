namespace Zerphin.RentACar.Application.Features.Users.GetUserStatistics;

public class GetUserStatisticsResponse
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public List<UserRoleStatistics> RoleStatistics { get; set; } = new();
    public int NewUsersLast30Days { get; set; }
}


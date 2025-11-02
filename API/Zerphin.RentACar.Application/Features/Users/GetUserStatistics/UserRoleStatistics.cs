using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Users.GetUserStatistics;

public class UserRoleStatistics
{
    public UserRole Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int Count { get; set; }
}


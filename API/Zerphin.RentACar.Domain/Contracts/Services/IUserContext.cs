namespace Zerphin.RentACar.Domain.Contracts.Services;

public interface IUserContext
{
    string? GetCurrentUserId();
    string? GetCurrentUserEmail();
    string? GetCurrentUserName();
    string GetCurrentUser();
}

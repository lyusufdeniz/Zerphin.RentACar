using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Domain.Contracts.Services;

public interface IAuthenticationService
{
    Task<string> GenerateJwtTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
    Task<bool> ValidateTokenAsync(string token);
    Task<User?> GetUserFromTokenAsync(string token);
    Task<bool> IsTokenExpiredAsync(string token);
    Task<string> RefreshTokenAsync(string refreshToken);
}

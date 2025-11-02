using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.Options;

namespace Zerphin.RentACar.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;

    public AuthenticationService(IOptions<JwtOptions> jwtOptions, IUserRepository userRepository, IRepository<RefreshToken> refreshTokenRepository)
    {
        _jwtOptions = jwtOptions.Value;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync()
    {
        return Guid.NewGuid().ToString();
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtOptions.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetUserFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return await _userRepository.GetByIdAsync(userId);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsTokenExpiredAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        // Find the refresh token in the database
        var tokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        
        if (tokenEntity == null)
        {
            throw new SecurityTokenException("Invalid refresh token.");
        }

        // Check if the refresh token is expired
        if (tokenEntity.IsExpired)
        {
            // Remove expired token from database
            await _refreshTokenRepository.DeleteAsync(tokenEntity);
            throw new SecurityTokenException("Refresh token has expired.");
        }

        // Get the user associated with the refresh token
        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId);
        if (user == null)
        {
            throw new SecurityTokenException("User not found for refresh token.");
        }

        // Generate a new JWT token
        var newJwtToken = await GenerateJwtTokenAsync(user);

        // Generate a new refresh token
        var newRefreshToken = await GenerateRefreshTokenAsync();

        // Update the refresh token in the database
        tokenEntity.Token = newRefreshToken;
        tokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
        tokenEntity.UpdatedAt = DateTime.UtcNow;
        
        await _refreshTokenRepository.UpdateAsync(tokenEntity);

        return newJwtToken;
    }

}

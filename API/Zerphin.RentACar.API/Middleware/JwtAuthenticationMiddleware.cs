using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Zerphin.RentACar.Domain.Options;
using Microsoft.Extensions.Options;

namespace Zerphin.RentACar.API.Middleware;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtOptions _jwtOptions;

    public JwtAuthenticationMiddleware(RequestDelegate next, IOptions<JwtOptions> jwtOptions)
    {
        _next = next;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractTokenFromHeader(context);
        
        if (!string.IsNullOrEmpty(token))
        {
            var principal = ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }

        await _next(context);
    }

    private string? ExtractTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(authHeader))
        {
            // Case-insensitive kontrol (Bearer, bearer, BEARER gibi)
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            // Eğer Bearer prefix'i yoksa, tüm header'ı token olarak kabul et
            // (bazı client'lar Bearer prefix'i eklemeden gönderebilir)
            return authHeader.Trim();
        }

        return null;
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtOptions.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ClockSkew = TimeSpan.Zero,
                // JWT claim'lerini .NET ClaimTypes'e map et
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            // Principal'in Identity'sine AuthenticationType ekle (IsAuthenticated için gerekli)
            // JWT token validate edildikten sonra AuthenticationType null olabilir
            if (principal?.Identity is ClaimsIdentity identity)
            {
                if (string.IsNullOrEmpty(identity.AuthenticationType))
                {
                    // NameIdentifier için ClaimTypes.NameIdentifier, Role için ClaimTypes.Role kullan
                    var claimsIdentity = new ClaimsIdentity(
                        identity.Claims, 
                        "Bearer", 
                        ClaimTypes.NameIdentifier, 
                        ClaimTypes.Role);
                    principal = new ClaimsPrincipal(claimsIdentity);
                }
            }
            
            return principal;
        }
        catch (Exception ex)
        {
            // Debug için exception detayları
            // Token validation başarısız olduğunda null dön
            // Gelecekte logger eklenebilir: _logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }
}

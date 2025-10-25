using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Zerphin.RentACar.Domain.Contracts.Services;

namespace Zerphin.RentACar.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }
        catch
        {
            // Ignore errors
        }

        return null;
    }

    public string? GetCurrentUserEmail()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            }
        }
        catch
        {
            // Ignore errors
        }

        return null;
    }

    public string? GetCurrentUserName()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            }
        }
        catch
        {
            // Ignore errors
        }

        return null;
    }

    public string GetCurrentUser()
    {
        // Try to get user ID first
        var userId = GetCurrentUserId();
        if (!string.IsNullOrEmpty(userId))
            return userId;

        // Try to get email
        var email = GetCurrentUserEmail();
        if (!string.IsNullOrEmpty(email))
            return email;

        // Try to get name
        var name = GetCurrentUserName();
        if (!string.IsNullOrEmpty(name))
            return name;

        // Return system user as fallback
        return "System";
    }
}

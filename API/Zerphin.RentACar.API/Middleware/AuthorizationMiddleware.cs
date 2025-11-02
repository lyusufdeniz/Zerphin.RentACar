using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using Zerphin.RentACar.Domain.Attributes;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.API.Middleware;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        // Check for RequireRole attribute
        var roleAttribute = endpoint.Metadata.GetMetadata<RequireRoleAttribute>();
        if (roleAttribute != null)
        {
            var authResult = await CheckRoleAuthorization(context, roleAttribute.Roles, userRepository);
            if (!authResult.IsSuccess)
            {
                context.Response.StatusCode = (int)authResult.Status;
                context.Response.ContentType = "application/json";
                
                var jsonResponse = JsonSerializer.Serialize(authResult);
                await context.Response.WriteAsync(jsonResponse);
                return;
            }
        }

        await _next(context);
    }

    private async Task<ServiceResult> CheckRoleAuthorization(HttpContext context, string[] requiredRoles, IUserRepository userRepository)
    {
        // Önce authentication durumunu kontrol et
        if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            return ServiceResult.Fail("User not authenticated.", System.Net.HttpStatusCode.Unauthorized);
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return ServiceResult.Fail("User not authenticated. Invalid token claims.", System.Net.HttpStatusCode.Unauthorized);
        }

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Fail("User not found.", System.Net.HttpStatusCode.Unauthorized);
        }

        var userRole = user.Role.ToString().ToLower();
        
        // Check if user has any of the required roles or higher
        var hasAccess = requiredRoles.Any(requiredRole => 
            HasRoleOrHigher(userRole, requiredRole.ToLower()));

        if (!hasAccess)
        {
            return ServiceResult.Fail($"Access denied. Required roles: {string.Join(", ", requiredRoles)}. Your role: {user.Role}", System.Net.HttpStatusCode.Forbidden);
        }

        return ServiceResult.Success();
    }

    private static bool HasRoleOrHigher(string userRole, string requiredRole)
    {
        // Role hierarchy: admin > manager > employee > customer
        var roleHierarchy = new Dictionary<string, int>
        {
            { "admin", 4 },
            { "manager", 3 },
            { "employee", 2 },
            { "customer", 1 }
        };

        var userLevel = roleHierarchy.GetValueOrDefault(userRole, 0);
        var requiredLevel = roleHierarchy.GetValueOrDefault(requiredRole, 0);

        return userLevel >= requiredLevel;
    }
}

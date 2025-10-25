using Zerphin.RentACar.API.Middleware;
using Zerphin.RentACar.Domain.Options;

namespace Zerphin.RentACar.API.Extensions;

public static class MiddlewareExtensions
{
  
    public static IApplicationBuilder AddAuthzMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<JwtAuthenticationMiddleware>();
        app.UseMiddleware<AuthorizationMiddleware>();
        return app;
    }
}

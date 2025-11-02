using Zerphin.RentACar.API.Middleware;


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

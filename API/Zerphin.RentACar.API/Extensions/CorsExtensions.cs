using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Zerphin.RentACar.API.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                            ?? new[] { "http://localhost:3000", "http://localhost:5173", "http://localhost:5174" };

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials(); // SPA'lar için credentials (cookies, authorization headers) desteği
            });
        });

        return services;
    }
}

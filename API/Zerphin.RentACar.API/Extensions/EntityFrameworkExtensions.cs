using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.API.Extensions;

public static class AddEntityFrameworkExtensions
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        services.AddDbContext<RentACarDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zerphin.RentACar.Domain.Options;

namespace Zerphin.RentACar.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));
        
        // Register JwtOptions as singleton for direct injection
        services.AddSingleton(provider =>
        {
            var options = new JwtOptions();
            configuration.GetSection(JwtOptions.SectionName).Bind(options);
            return options;
        });
        
        return services;
    }
}


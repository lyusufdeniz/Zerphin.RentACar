using Microsoft.Extensions.DependencyInjection;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Infrastructure.Data.Interceptors;
using Zerphin.RentACar.Infrastructure.Repositories;
using Zerphin.RentACar.Infrastructure.Services;

namespace Zerphin.RentACar.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Interceptors
        services.AddScoped<AuditableInterceptor>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Specific Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IInsuranceRepository, InsuranceRepository>();

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        // Background Services
        services.AddHostedService<Services.RentalExpirationService>();

        return services;
    }
}

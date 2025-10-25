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

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<Role>, Repository<Role>>();
        services.AddScoped<IRepository<RefreshToken>, Repository<RefreshToken>>();
        services.AddScoped<IRepository<Customer>, Repository<Customer>>();
        services.AddScoped<IRepository<Vehicle>, Repository<Vehicle>>();
        services.AddScoped<IRepository<Rental>, Repository<Rental>>();
        services.AddScoped<IRepository<Payment>, Repository<Payment>>();
        services.AddScoped<IRepository<Invoice>, Repository<Invoice>>();
        services.AddScoped<IRepository<Insurance>, Repository<Insurance>>();
        services.AddScoped<IRepository<Location>, Repository<Location>>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IUserContext, UserContext>();
        // TODO: Add EmailService and NotificationService implementations when needed
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}

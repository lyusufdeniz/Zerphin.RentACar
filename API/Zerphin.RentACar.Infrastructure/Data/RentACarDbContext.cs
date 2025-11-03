using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Infrastructure.Data.Interceptors;

namespace Zerphin.RentACar.Infrastructure.Data;

public class RentACarDbContext : DbContext
{
    private readonly AuditableInterceptor _auditableInterceptor;

    public RentACarDbContext(DbContextOptions<RentACarDbContext> options, AuditableInterceptor auditableInterceptor) : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Insurance> Insurances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfraAssembly).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor);
        
        // Suppress pending model changes warning
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        base.OnConfiguring(optionsBuilder);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Seed;

public class RoleSeed : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "System Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Role
            {
                Id = 2,
                Name = "Manager",
                Description = "Branch Manager",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Role
            {
                Id = 3,
                Name = "Employee",
                Description = "Rental Employee",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Role
            {
                Id = 4,
                Name = "Customer",
                Description = "Rental Customer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );
    }
}

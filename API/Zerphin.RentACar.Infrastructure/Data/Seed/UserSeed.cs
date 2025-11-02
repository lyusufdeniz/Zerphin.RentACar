using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;
using Zerphin.RentACar.Infrastructure.Services;

namespace Zerphin.RentACar.Infrastructure.Data.Seed;

public class UserSeed : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@zerphinrentacar.com",
                PhoneNumber = "+905551234567",
                PasswordHash = PasswordService.HashPasswordStatic("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
                Address = "Istanbul, Turkey",
                IdentityNumber = "12345678901",
                BirthDate = new DateTime(1985, 1, 1),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                FirstName = "Manager",
                LastName = "User",
                Email = "manager@zerphinrentacar.com",
                PhoneNumber = "+905551234568",
                PasswordHash = PasswordService.HashPasswordStatic("Manager123!"),
                Role = UserRole.Manager,
                IsActive = true,
                Address = "Ankara, Turkey",
                IdentityNumber = "12345678902",
                BirthDate = new DateTime(1988, 5, 15),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                FirstName = "Employee",
                LastName = "User",
                Email = "employee@zerphinrentacar.com",
                PhoneNumber = "+905551234569",
                PasswordHash = PasswordService.HashPasswordStatic("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                Address = "Izmir, Turkey",
                IdentityNumber = "12345678903",
                BirthDate = new DateTime(1990, 8, 20),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                PhoneNumber = "+905551234570",
                PasswordHash = PasswordService.HashPasswordStatic("Customer123!"),
                Role = UserRole.Customer,
                IsActive = true,
                Address = "Bursa, Turkey",
                IdentityNumber = "12345678904",
                BirthDate = new DateTime(1992, 12, 10),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@email.com",
                PhoneNumber = "+905551234571",
                PasswordHash = PasswordService.HashPasswordStatic("Customer123!"),
                Role = UserRole.Customer,
                IsActive = true,
                Address = "Antalya, Turkey",
                IdentityNumber = "12345678905",
                BirthDate = new DateTime(1995, 3, 25),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );
    }
}

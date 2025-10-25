using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Infrastructure.Services;

namespace Zerphin.RentACar.Infrastructure.Data.Seed;

public class UserSeed : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(
            new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@zerphinrentacar.com",
                PhoneNumber = "+905551234567",
                PasswordHash = PasswordService.HashPasswordStatic("Admin123!"),
                RoleId = 1,
                IsActive = true,
                Address = "Istanbul, Turkey",
                IdentityNumber = "12345678901",
                BirthDate = new DateTime(1985, 1, 1),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = 2,
                FirstName = "Manager",
                LastName = "User",
                Email = "manager@zerphinrentacar.com",
                PhoneNumber = "+905551234568",
                PasswordHash = PasswordService.HashPasswordStatic("Manager123!"),
                RoleId = 2,
                IsActive = true,
                Address = "Ankara, Turkey",
                IdentityNumber = "12345678902",
                BirthDate = new DateTime(1988, 5, 15),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = 3,
                FirstName = "Employee",
                LastName = "User",
                Email = "employee@zerphinrentacar.com",
                PhoneNumber = "+905551234569",
                PasswordHash = PasswordService.HashPasswordStatic("Employee123!"),
                RoleId = 3,
                IsActive = true,
                Address = "Izmir, Turkey",
                IdentityNumber = "12345678903",
                BirthDate = new DateTime(1990, 8, 20),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = 4,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                PhoneNumber = "+905551234570",
                PasswordHash = PasswordService.HashPasswordStatic("Customer123!"),
                RoleId = 4,
                IsActive = true,
                Address = "Bursa, Turkey",
                IdentityNumber = "12345678904",
                BirthDate = new DateTime(1992, 12, 10),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new User
            {
                Id = 5,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@email.com",
                PhoneNumber = "+905551234571",
                PasswordHash = PasswordService.HashPasswordStatic("Customer123!"),
                RoleId = 4,
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

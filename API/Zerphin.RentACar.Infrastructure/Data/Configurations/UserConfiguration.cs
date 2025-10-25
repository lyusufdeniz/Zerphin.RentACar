using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(e => e.RoleId)
            .IsRequired();
            
        builder.HasOne(e => e.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Property(e => e.Address)
            .HasMaxLength(500);
            
        builder.Property(e => e.IdentityNumber)
            .HasMaxLength(20);
            
        builder.HasIndex(e => e.Email)
            .IsUnique();
            
        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);
    }
}

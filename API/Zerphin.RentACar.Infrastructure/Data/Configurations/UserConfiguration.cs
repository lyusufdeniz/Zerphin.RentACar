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
            
        builder.Property(e => e.Role)
            .IsRequired()
            .HasConversion<int>(); // Store enum as int in database
            
        builder.Property(e => e.Address)
            .HasMaxLength(500);
            
        builder.Property(e => e.IdentityNumber)
            .HasMaxLength(20);
            
        builder.HasIndex(e => e.Email)
            .IsUnique();
            
        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);
            
        // Customer Details
        builder.Property(e => e.LicenseNumber)
            .HasMaxLength(20);
            
        builder.Property(e => e.LicenseClass)
            .HasMaxLength(50);
            
        builder.Property(e => e.EmergencyContactName)
            .HasMaxLength(100);
            
        builder.Property(e => e.EmergencyContactPhone)
            .HasMaxLength(20);
            
        builder.Property(e => e.SpecialNotes)
            .HasMaxLength(500);
            
        builder.Property(e => e.VerificationDocument)
            .HasMaxLength(100);
            
        builder.Property(e => e.InsuranceCompany)
            .HasMaxLength(100);
            
        builder.Property(e => e.InsurancePolicyNumber)
            .HasMaxLength(50);
            
        builder.Property(e => e.IsVerified)
            .HasDefaultValue(false);
            
        builder.Property(e => e.CreditScore)
            .HasDefaultValue(0);
            
        builder.Property(e => e.HasInsurance)
            .HasDefaultValue(false);
    }
}

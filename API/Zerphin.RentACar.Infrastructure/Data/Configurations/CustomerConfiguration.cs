using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.LicenseNumber)
            .IsRequired()
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
            
        builder.HasOne(e => e.User)
            .WithOne(e => e.Customer)
            .HasForeignKey<Customer>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

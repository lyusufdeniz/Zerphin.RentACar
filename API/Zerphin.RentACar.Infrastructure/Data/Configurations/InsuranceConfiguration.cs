using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class InsuranceConfiguration : IEntityTypeConfiguration<Insurance>
{
    public void Configure(EntityTypeBuilder<Insurance> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.InsuranceCompany)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.PolicyNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.StartDate)
            .IsRequired();
            
        builder.Property(e => e.EndDate)
            .IsRequired();
            
        builder.Property(e => e.PremiumAmount)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(e => e.CoverageType)
            .HasMaxLength(50);
            
        builder.Property(e => e.CoverageLimit)
            .HasPrecision(10, 2);
            
        builder.Property(e => e.Deductible)
            .HasPrecision(10, 2);
            
        builder.Property(e => e.CoverageDetails)
            .HasMaxLength(1000);
            
        builder.Property(e => e.ContactInfo)
            .HasMaxLength(500);
            
        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);
            
        builder.HasOne(e => e.Vehicle)
            .WithOne(e => e.Insurance)
            .HasForeignKey<Insurance>(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

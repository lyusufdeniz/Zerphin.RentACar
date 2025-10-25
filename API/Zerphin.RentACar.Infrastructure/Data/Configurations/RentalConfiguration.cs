using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.StartDate)
            .IsRequired();
            
        builder.Property(e => e.EndDate)
            .IsRequired();
            
        builder.Property(e => e.DailyRate)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(e => e.TotalAmount)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(e => e.LateFee)
            .HasPrecision(10, 2);
            
        builder.Property(e => e.DamageFee)
            .HasPrecision(10, 2);
            
        builder.Property(e => e.Notes)
            .HasMaxLength(1000);
            
        builder.Property(e => e.PickupLocation)
            .HasMaxLength(500);
            
        builder.Property(e => e.ReturnLocation)
            .HasMaxLength(500);
            
        builder.Property(e => e.Status)
            .HasDefaultValue(RentalStatus.Pending);
            
        builder.HasOne(e => e.Customer)
            .WithMany(e => e.Rentals)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(e => e.Vehicle)
            .WithMany(e => e.Rentals)
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

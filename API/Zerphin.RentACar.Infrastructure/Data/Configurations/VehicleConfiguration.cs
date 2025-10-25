using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Brand)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Model)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.LicensePlate)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.Color)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.FuelType)
            .HasMaxLength(50);
            
        builder.Property(e => e.Transmission)
            .HasMaxLength(50);
            
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
            
        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);
            
        builder.Property(e => e.DailyRentalPrice)
            .HasPrecision(10, 2);
            
        builder.Property(e => e.Status)
            .HasDefaultValue(VehicleStatus.Available);
            
        builder.Property(e => e.HasAirConditioning)
            .HasDefaultValue(false);
            
        builder.Property(e => e.HasGPS)
            .HasDefaultValue(false);
            
        builder.Property(e => e.HasBluetooth)
            .HasDefaultValue(false);
            
        builder.HasIndex(e => e.LicensePlate)
            .IsUnique();
            
        builder.HasOne(e => e.Location)
            .WithMany(e => e.Vehicles)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.State)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.PostalCode)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.Country)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);
            
        builder.Property(e => e.Email)
            .HasMaxLength(255);
            
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
            
        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);
            
        builder.Property(e => e.IsPickupLocation)
            .HasDefaultValue(true);
            
        builder.Property(e => e.IsReturnLocation)
            .HasDefaultValue(true);
    }
}

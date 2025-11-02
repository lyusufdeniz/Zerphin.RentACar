using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.InvoiceDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(e => e.DueDate)
            .IsRequired();
            
        builder.Property(e => e.SubTotal)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(e => e.TaxAmount)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(e => e.TotalAmount)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(e => e.TaxNumber)
            .HasMaxLength(20);
            
        builder.Property(e => e.BillingAddress)
            .HasMaxLength(500);
            
        builder.Property(e => e.CustomerName)
            .HasMaxLength(100);
            
        builder.Property(e => e.CustomerEmail)
            .HasMaxLength(255);
            
        builder.Property(e => e.CustomerTaxNumber)
            .HasMaxLength(20);
            
        builder.Property(e => e.Notes)
            .HasMaxLength(1000);
            
        builder.Property(e => e.IsPaid)
            .HasDefaultValue(false);
            
        builder.HasIndex(e => e.InvoiceNumber)
            .IsUnique();
            
        builder.HasOne(e => e.Rental)
            .WithOne(e => e.Invoice)
            .HasForeignKey<Invoice>(e => e.RentalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

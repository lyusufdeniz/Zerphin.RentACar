using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Amount)
            .IsRequired()
            .HasPrecision(10, 2);
            
        builder.Property(e => e.Method)
            .IsRequired();
            
        builder.Property(e => e.Status)
            .IsRequired();
            
        builder.Property(e => e.PaymentDate)
            .IsRequired();
            
        builder.Property(e => e.TransactionId)
            .HasMaxLength(100);
            
        builder.Property(e => e.ReferenceNumber)
            .HasMaxLength(100);
            
        builder.Property(e => e.Notes)
            .HasMaxLength(500);
            
        builder.Property(e => e.FailureReason)
            .HasMaxLength(1000);
            
        builder.Property(e => e.CardLastFourDigits)
            .HasMaxLength(50);
            
        builder.Property(e => e.BankName)
            .HasMaxLength(100);
            
        builder.Property(e => e.Status)
            .HasDefaultValue(PaymentStatus.Pending);
            
        builder.HasOne(e => e.Rental)
            .WithMany(e => e.Payments)
            .HasForeignKey(e => e.RentalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

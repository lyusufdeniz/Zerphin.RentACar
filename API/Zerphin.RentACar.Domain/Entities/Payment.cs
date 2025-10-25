namespace Zerphin.RentACar.Domain.Entities;

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }
    
    public PaymentMethod Method { get; set; }
    
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public DateTime PaymentDate { get; set; }
    
    public DateTime? ProcessedDate { get; set; }
    
    public string? TransactionId { get; set; }
    
    public string? ReferenceNumber { get; set; }
    
    public string? Notes { get; set; }
    
    public string? FailureReason { get; set; }
    
    public string? CardLastFourDigits { get; set; }
    
    public string? BankName { get; set; }
    
    public int RentalId { get; set; }
    
    public virtual Rental Rental { get; set; } = null!;
}

namespace Zerphin.RentACar.Domain.Entities;

public class Rental : BaseEntity
{
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime? ActualReturnDate { get; set; }
    
    public decimal DailyRate { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public decimal? LateFee { get; set; }
    
    public decimal? DamageFee { get; set; }
    
    public RentalStatus Status { get; set; } = RentalStatus.Pending;
    
    public string? Notes { get; set; }
    
    public string? PickupLocation { get; set; }
    
    public string? ReturnLocation { get; set; }
    
    public int? KmAtStart { get; set; }
    
    public int? KmAtReturn { get; set; }
    
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    
    public virtual User Customer { get; set; } = null!;
    public virtual Vehicle Vehicle { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual Invoice? Invoice { get; set; }
}

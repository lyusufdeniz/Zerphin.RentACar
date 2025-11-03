using Zerphin.RentACar.Domain.ValueObjects;

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
    
    public RentalStatus Status { get; set; } = RentalStatus.Active;
    
    public string? Notes { get; set; }
    
    public string? PickupLocation { get; set; }
    
    public string? ReturnLocation { get; set; }
    
    public int? KmAtStart { get; set; }
    
    public int? KmAtReturn { get; set; }
    
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    
    public virtual User Customer { get; set; } = null!;
    public virtual Vehicle Vehicle { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

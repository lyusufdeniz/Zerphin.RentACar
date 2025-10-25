using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string Brand { get; set; } = string.Empty;
    
    public string Model { get; set; } = string.Empty;
    
    public string LicensePlate { get; set; } = string.Empty;
    
    public int Year { get; set; }
    
    public string Color { get; set; } = string.Empty;
    
    public VehicleCategory Category { get; set; }
    
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
    
    public decimal DailyRentalPrice { get; set; }
    
    public int SeatingCapacity { get; set; }
    
    public string? FuelType { get; set; }
    
    public string? Transmission { get; set; }
    
    public int? Km { get; set; }
    
    public string? Description { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public bool HasAirConditioning { get; set; } = false;
    
    public bool HasGPS { get; set; } = false;
    
    public bool HasBluetooth { get; set; } = false;
    
    public int? LocationId { get; set; }
    
    public virtual Location? Location { get; set; }
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public virtual Insurance? Insurance { get; set; }
}

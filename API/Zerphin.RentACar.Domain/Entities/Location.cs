namespace Zerphin.RentACar.Domain.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public string PostalCode { get; set; } = string.Empty;
    
    public string Country { get; set; } = string.Empty;
    
    public decimal? Latitude { get; set; }
    
    public decimal? Longitude { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? Email { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsPickupLocation { get; set; } = true;
    
    public bool IsReturnLocation { get; set; } = true;
    
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}

namespace Zerphin.RentACar.Domain.Entities;

public class Insurance : BaseEntity
{
    public string InsuranceCompany { get; set; } = string.Empty;
    
    public string PolicyNumber { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public decimal PremiumAmount { get; set; }
    
    public string? CoverageType { get; set; }
    
    public decimal? CoverageLimit { get; set; }
    
    public decimal? Deductible { get; set; }
    
    public string? CoverageDetails { get; set; }
    
    public string? ContactInfo { get; set; }
    
    public Guid VehicleId { get; set; }
    
    public virtual Vehicle Vehicle { get; set; } = null!;
}

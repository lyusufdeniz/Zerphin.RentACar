namespace Zerphin.RentACar.Domain.Entities;

public class Customer : BaseEntity
{
    public string LicenseNumber { get; set; } = string.Empty;
    
    public DateTime LicenseExpiryDate { get; set; }
    
    public string? LicenseClass { get; set; }
    
    public string? EmergencyContactName { get; set; }
    
    public string? EmergencyContactPhone { get; set; }
    
    public string? SpecialNotes { get; set; }
    
    public bool IsVerified { get; set; } = false;
    
    public DateTime? VerificationDate { get; set; }
    
    public string? VerificationDocument { get; set; }
    
    public int CreditScore { get; set; } = 0;
    
    public bool HasInsurance { get; set; } = false;
    
    public string? InsuranceCompany { get; set; }
    
    public string? InsurancePolicyNumber { get; set; }
    
    public int UserId { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}

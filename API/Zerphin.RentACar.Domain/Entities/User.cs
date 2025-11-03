using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public UserRole Role { get; set; } = UserRole.Customer;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? LastLoginAt { get; set; }
    
    public string? Address { get; set; }
    
    public string? IdentityNumber { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    // Customer Details
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
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
    
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

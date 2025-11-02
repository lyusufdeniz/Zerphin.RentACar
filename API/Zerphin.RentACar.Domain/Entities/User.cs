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
    
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public virtual Customer? Customer { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

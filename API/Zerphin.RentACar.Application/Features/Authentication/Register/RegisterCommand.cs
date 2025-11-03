using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Authentication.Register;

public class RegisterCommand : IRequest<ServiceResult<RegisterResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? IdentityNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer; // Default to Customer role
    
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
}

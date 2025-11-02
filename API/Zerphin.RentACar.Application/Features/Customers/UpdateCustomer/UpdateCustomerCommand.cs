using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Customers.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<ServiceResult<UpdateCustomerResponse>>
{
    public Guid Id { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiryDate { get; set; }
    public string? LicenseClass { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? SpecialNotes { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerificationDate { get; set; }
    public string? VerificationDocument { get; set; }
    public int CreditScore { get; set; }
    public bool HasInsurance { get; set; }
    public string? InsuranceCompany { get; set; }
    public string? InsurancePolicyNumber { get; set; }
}


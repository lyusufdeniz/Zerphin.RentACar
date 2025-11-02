namespace Zerphin.RentACar.Application.Features.Customers.SearchCustomers;

public class SearchCustomersResponse
{
    public List<CustomerDto> Customers { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public class CustomerDto
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
        public Guid UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
    }
}


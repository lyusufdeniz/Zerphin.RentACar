namespace Zerphin.RentACar.Application.Features.Insurances.SearchInsurances;

public class SearchInsurancesResponse
{
    public List<InsuranceDto> Insurances { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public class InsuranceDto
    {
        public Guid Id { get; set; }
        public string InsuranceCompany { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PremiumAmount { get; set; }
        public string? CoverageType { get; set; }
        public decimal? CoverageLimit { get; set; }
        public decimal? Deductible { get; set; }
        public string? CoverageDetails { get; set; }
        public bool IsActive { get; set; }
        public string? ContactInfo { get; set; }
        public Guid VehicleId { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleModel { get; set; }
        public string? VehicleLicensePlate { get; set; }
    }
}



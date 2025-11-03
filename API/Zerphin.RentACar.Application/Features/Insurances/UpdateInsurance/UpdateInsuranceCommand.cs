using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Insurances.UpdateInsurance;

public class UpdateInsuranceCommand : IRequest<ServiceResult<UpdateInsuranceResponse>>
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
    public string? ContactInfo { get; set; }
}




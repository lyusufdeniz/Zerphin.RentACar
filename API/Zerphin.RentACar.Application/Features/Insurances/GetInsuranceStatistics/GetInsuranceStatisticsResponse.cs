namespace Zerphin.RentACar.Application.Features.Insurances.GetInsuranceStatistics;

public class GetInsuranceStatisticsResponse
{
    public int TotalInsurances { get; set; }
    public int ActiveInsurances { get; set; }
    public int ExpiredInsurances { get; set; }
    public int ExpiringSoonInsurances { get; set; }
    public decimal TotalPremiumAmount { get; set; }
}


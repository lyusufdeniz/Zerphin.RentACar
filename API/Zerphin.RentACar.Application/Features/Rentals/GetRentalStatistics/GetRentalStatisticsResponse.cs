namespace Zerphin.RentACar.Application.Features.Rentals.GetRentalStatistics;

public class GetRentalStatisticsResponse
{
    public int TotalRentals { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRentalAmount { get; set; }
    public double AverageRentalDays { get; set; }
    
    public List<RentalStatusStatistics> StatusStatistics { get; set; } = new();
    public List<RentalMonthlyStatistics> MonthlyStatistics { get; set; } = new();
    
    public int ActiveRentals { get; set; }
    public int CompletedRentals { get; set; }
    public int CancelledRentals { get; set; }
}


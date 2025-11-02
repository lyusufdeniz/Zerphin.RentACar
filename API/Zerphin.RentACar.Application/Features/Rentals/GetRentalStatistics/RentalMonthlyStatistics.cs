namespace Zerphin.RentACar.Application.Features.Rentals.GetRentalStatistics;

public class RentalMonthlyStatistics
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalRevenue { get; set; }
}


namespace Zerphin.RentACar.Application.Features.Customers.GetCustomerStatistics;

public class GetCustomerStatisticsResponse
{
    public int TotalCustomers { get; set; }
    public int VerifiedCustomers { get; set; }
    public int UnverifiedCustomers { get; set; }
    public int CustomersWithInsurance { get; set; }
    public int NewCustomersLast30Days { get; set; }
    public double AverageCreditScore { get; set; }
}


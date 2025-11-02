namespace Zerphin.RentACar.Application.Features.Invoices.GetInvoiceStatistics;

public class GetInvoiceStatisticsResponse
{
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int UnpaidInvoices { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal UnpaidAmount { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal OverdueAmount { get; set; }
}


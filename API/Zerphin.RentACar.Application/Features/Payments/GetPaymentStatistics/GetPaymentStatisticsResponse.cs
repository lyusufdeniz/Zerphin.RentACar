namespace Zerphin.RentACar.Application.Features.Payments.GetPaymentStatistics;

public class GetPaymentStatisticsResponse
{
    public int TotalPayments { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CompletedAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public decimal FailedAmount { get; set; }
    
    public List<PaymentStatusStatistics> StatusStatistics { get; set; } = new();
    public List<PaymentMethodStatistics> MethodStatistics { get; set; } = new();
}


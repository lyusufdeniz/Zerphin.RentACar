using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Payments.GetPaymentStatistics;

public class PaymentMethodStatistics
{
    public PaymentMethod Method { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}


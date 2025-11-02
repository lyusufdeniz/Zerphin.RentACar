using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Payments.GetPaymentById;

public class GetPaymentByIdResponse
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? FailureReason { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? BankName { get; set; }
    public Guid RentalId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
}



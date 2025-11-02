using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Payments.CreatePayment;

public class CreatePaymentCommand : IRequest<ServiceResult<CreatePaymentResponse>>
{
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedDate { get; set; }
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? FailureReason { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? BankName { get; set; }
    public Guid RentalId { get; set; }
}



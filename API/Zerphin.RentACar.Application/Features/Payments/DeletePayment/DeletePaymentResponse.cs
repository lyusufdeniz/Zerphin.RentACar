namespace Zerphin.RentACar.Application.Features.Payments.DeletePayment;

public class DeletePaymentResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = "Payment deleted successfully.";
}


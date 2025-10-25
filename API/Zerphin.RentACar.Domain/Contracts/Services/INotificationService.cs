namespace Zerphin.RentACar.Domain.Contracts.Services;

public interface INotificationService
{
    Task SendWelcomeEmailAsync(string email, string firstName);
    Task SendPasswordResetEmailAsync(string email, string resetToken);
    Task SendRentalConfirmationEmailAsync(string email, string customerName, string vehicleInfo, DateTime startDate, DateTime endDate);
    Task SendRentalReminderEmailAsync(string email, string customerName, DateTime returnDate);
    Task SendPaymentConfirmationEmailAsync(string email, string customerName, decimal amount, string paymentMethod);
    Task SendRentalExpiredEmailAsync(string email, string customerName, string vehicleInfo);
    Task SendSystemNotificationAsync(string email, string subject, string message);
}

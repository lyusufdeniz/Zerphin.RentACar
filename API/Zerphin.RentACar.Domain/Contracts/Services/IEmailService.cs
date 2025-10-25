namespace Zerphin.RentACar.Domain.Contracts.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailAsync(string to, string cc, string subject, string body);
    Task SendEmailAsync(string to, string cc, string bcc, string subject, string body);
    Task SendHtmlEmailAsync(string to, string subject, string htmlBody);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
    Task<bool> IsEmailValidAsync(string email);
    Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body);
}

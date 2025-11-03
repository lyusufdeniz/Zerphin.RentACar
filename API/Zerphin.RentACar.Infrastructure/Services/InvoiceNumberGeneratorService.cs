using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;

namespace Zerphin.RentACar.Infrastructure.Services;

public class InvoiceNumberGeneratorService : IInvoiceNumberGenerator
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceNumberGeneratorService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        // Format: INV-YYYYMMDD-XXXX (e.g., INV-20240115-0001)
        var today = DateTime.UtcNow;
        var datePrefix = today.ToString("yyyyMMdd");
        var prefix = $"INV-{datePrefix}";

        // Find the last invoice number for today
        var todayInvoices = await _invoiceRepository.GetByDateRangeAsync(today.Date, today.Date.AddDays(1));
        var todayCount = todayInvoices.Count();
        var sequenceNumber = (todayCount + 1).ToString("D4"); // 4 digit with leading zeros

        var invoiceNumber = $"{prefix}-{sequenceNumber}";

        // Double check if it exists (race condition için)
        if (await _invoiceRepository.IsInvoiceNumberExistsAsync(invoiceNumber))
        {
            // Retry with incremented number
            var retryCount = 0;
            do
            {
                retryCount++;
                sequenceNumber = (todayCount + 1 + retryCount).ToString("D4");
                invoiceNumber = $"{prefix}-{sequenceNumber}";
            } while (await _invoiceRepository.IsInvoiceNumberExistsAsync(invoiceNumber) && retryCount < 100);
        }

        return invoiceNumber;
    }
}


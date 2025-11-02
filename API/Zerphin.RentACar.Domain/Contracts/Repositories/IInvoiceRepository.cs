using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<Invoice?> GetByRentalIdAsync(Guid rentalId);
    Task<IEnumerable<Invoice>> GetPaidInvoicesAsync();
    Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> IsInvoiceNumberExistsAsync(string invoiceNumber);
}

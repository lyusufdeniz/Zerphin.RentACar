using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByRentalIdAsync(Guid rentalId);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);
    Task<IEnumerable<Payment>> GetByMethodAsync(PaymentMethod method);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Payment?> GetByTransactionIdAsync(string transactionId);
    Task<decimal> GetTotalAmountByRentalIdAsync(Guid rentalId);
}

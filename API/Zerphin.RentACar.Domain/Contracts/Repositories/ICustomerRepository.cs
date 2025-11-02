using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByUserIdAsync(Guid userId);
    Task<Customer?> GetByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<Customer>> GetVerifiedCustomersAsync();
    Task<IEnumerable<Customer>> GetByCreditScoreRangeAsync(int minScore, int maxScore);
    Task<bool> IsLicenseNumberExistsAsync(string licenseNumber);
}

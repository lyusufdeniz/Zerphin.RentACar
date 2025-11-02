using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IInsuranceRepository : IRepository<Insurance>
{
    Task<Insurance?> GetByVehicleIdAsync(Guid vehicleId);
    Task<Insurance?> GetByPolicyNumberAsync(string policyNumber);
    Task<IEnumerable<Insurance>> GetActiveInsurancesAsync();
    Task<IEnumerable<Insurance>> GetExpiredInsurancesAsync();
    Task<IEnumerable<Insurance>> GetExpiringSoonAsync(int days);
    Task<bool> IsPolicyNumberExistsAsync(string policyNumber);
}

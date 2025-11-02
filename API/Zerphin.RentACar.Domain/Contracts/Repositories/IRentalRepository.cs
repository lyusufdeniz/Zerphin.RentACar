using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Rental>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status);
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Rental>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Rental>> GetOverdueRentalsAsync();
    Task<Rental?> GetActiveRentalByVehicleIdAsync(Guid vehicleId);
    
    Task<PagedResult<Rental>> GetPagedByCustomerIdAsync(Guid customerId, int pageNumber, int pageSize);
    Task<PagedResult<Rental>> GetPagedByVehicleIdAsync(Guid vehicleId, int pageNumber, int pageSize);
    Task<PagedResult<Rental>> GetPagedByStatusAsync(RentalStatus status, int pageNumber, int pageSize);
    Task<PagedResult<Rental>> GetPagedActiveRentalsAsync(int pageNumber, int pageSize);
    Task<PagedResult<Rental>> GetPagedByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    Task<PagedResult<Rental>> GetPagedOverdueRentalsAsync(int pageNumber, int pageSize);
}

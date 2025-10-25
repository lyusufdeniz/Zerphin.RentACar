using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Vehicle>> GetByCategoryAsync(VehicleCategory category);
    Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status);
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
    Task<IEnumerable<Vehicle>> GetByLocationAsync(int locationId);
    Task<IEnumerable<Vehicle>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<bool> IsLicensePlateExistsAsync(string licensePlate);
    
    Task<PagedResult<Vehicle>> GetPagedByCategoryAsync(VehicleCategory category, int pageNumber, int pageSize);
    Task<PagedResult<Vehicle>> GetPagedByStatusAsync(VehicleStatus status, int pageNumber, int pageSize);
    Task<PagedResult<Vehicle>> GetPagedAvailableVehiclesAsync(int pageNumber, int pageSize);
    Task<PagedResult<Vehicle>> GetPagedByLocationAsync(int locationId, int pageNumber, int pageSize);
    Task<PagedResult<Vehicle>> GetPagedByPriceRangeAsync(decimal minPrice, decimal maxPrice, int pageNumber, int pageSize);
}

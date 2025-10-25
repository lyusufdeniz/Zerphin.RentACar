using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> GetActiveLocationsAsync();
    Task<IEnumerable<Location>> GetPickupLocationsAsync();
    Task<IEnumerable<Location>> GetReturnLocationsAsync();
    Task<IEnumerable<Location>> GetByCityAsync(string city);
    Task<Location?> GetByCoordinatesAsync(decimal latitude, decimal longitude);
}

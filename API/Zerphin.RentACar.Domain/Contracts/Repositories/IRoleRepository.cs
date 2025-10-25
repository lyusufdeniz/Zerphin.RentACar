using System.Linq.Expressions;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetActiveRolesAsync();
    Task<bool> IsNameExistsAsync(string name);
    Task<bool> IsNameExistsAsync(string name, int excludeId);

    Task<PagedResult<Role>> GetPagedActiveRolesAsync(int pageNumber, int pageSize);
    Task<PagedResult<Role>> GetPagedRolesAsync(int pageNumber, int pageSize, Expression<Func<Role, object>> orderBy, bool isDescending = false);
}

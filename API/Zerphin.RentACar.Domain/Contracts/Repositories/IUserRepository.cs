using System.Linq.Expressions;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.Common;

namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdentityNumberAsync(string identityNumber);
    Task<IEnumerable<User>> GetByRoleAsync(string role);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> IsEmailExistsAsync(string email);
    Task<bool> IsIdentityNumberExistsAsync(string identityNumber);
    
    Task<PagedResult<User>> GetPagedByRoleAsync(string role, int pageNumber, int pageSize);
    Task<PagedResult<User>> GetPagedActiveUsersAsync(int pageNumber, int pageSize);
    Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, Expression<Func<User, object>> orderBy, bool isDescending = false);
    
    // Business logic methods
    Task<bool> ActivateUserAsync(int userId);
    Task<bool> DeactivateUserAsync(int userId);
}

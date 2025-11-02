using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RentACarDbContext _context;

    public UserRepository(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<User, bool>>? predicate = null)
    {
        var query = _context.Users.Where(u => !u.IsDeleted);
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query.CountAsync();
    }

    public async Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<User, bool>> predicate)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted)
            .Where(predicate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<User, bool>> predicate, Expression<Func<User, object>> orderBy)
    {
        return await GetPagedAsync(pageNumber, pageSize, predicate, orderBy, false);
    }

    public async Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<User, bool>> predicate, Expression<Func<User, object>> orderBy, bool isDescending)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted)
            .Where(predicate);

        query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<User> AddAsync(User entity)
    {
        _context.Users.Add(entity);
        return entity;
    }

    public async Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities)
    {
        var entityList = entities.ToList();
        _context.Users.AddRange(entityList);
        return entityList;
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
    }

    public async Task UpdateRangeAsync(IEnumerable<User> entities)
    {
        var entityList = entities.ToList();
        _context.Users.UpdateRange(entityList);
    }

    public async Task DeleteAsync(User entity)
    {
        entity.IsDeleted = true;
        await UpdateAsync(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<User> entities)
    {
        var entityList = entities.ToList();
        foreach (var entity in entityList)
        {
            entity.IsDeleted = true;
        }
        
        await UpdateRangeAsync(entityList);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<User?> GetByIdentityNumberAsync(string identityNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.IdentityNumber == identityNumber && !u.IsDeleted);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role)
    {
        if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var userRole))
        {
            return Enumerable.Empty<User>();
        }

        return await _context.Users
            .Where(u => !u.IsDeleted && u.Role == userRole)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted && u.IsActive)
            .ToListAsync();
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<bool> IsIdentityNumberExistsAsync(string identityNumber)
    {
        return await _context.Users
            .AnyAsync(u => u.IdentityNumber == identityNumber && !u.IsDeleted);
    }

    public async Task<PagedResult<User>> GetPagedByRoleAsync(string role, int pageNumber, int pageSize)
    {
        if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var userRole))
        {
            return new PagedResult<User>(new List<User>(), 0, pageNumber, pageSize);
        }

        return await GetPagedAsync(pageNumber, pageSize, u => u.Role == userRole);
    }

    public async Task<PagedResult<User>> GetPagedActiveUsersAsync(int pageNumber, int pageSize)
    {
        return await GetPagedAsync(pageNumber, pageSize, u => u.IsActive);
    }

    public async Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, Expression<Func<User, object>> orderBy, bool isDescending = false)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted);

        query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(items, totalCount, pageNumber, pageSize);
    }

    // Business logic methods - password handling moved to handlers

    public async Task<bool> ActivateUserAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.IsActive = true;
        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.IsActive = false;
        await UpdateAsync(user);
        return true;
    }

    // Password validation moved to handlers

  
}

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RentACarDbContext _context;
    private readonly DbSet<Role> _dbSet;

    public RoleRepository(RentACarDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Role>();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _dbSet.Where(r => !r.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(r => !r.IsDeleted).ToListAsync();
    }

    public async Task<Role?> FirstOrDefaultAsync(Expression<Func<Role, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(r => !r.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<Role, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(r => !r.IsDeleted).AnyAsync();
    }

    public async Task<int> CountAsync(Expression<Func<Role, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.Where(r => !r.IsDeleted).CountAsync();
        
        return await _dbSet.Where(predicate).Where(r => !r.IsDeleted).CountAsync();
    }

    public async Task<PagedResult<Role>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _dbSet.Where(r => !r.IsDeleted);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Role>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Role>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Role, bool>> predicate)
    {
        var query = _dbSet.Where(predicate).Where(r => !r.IsDeleted);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Role>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Role>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Role, bool>> predicate, Expression<Func<Role, object>> orderBy)
    {
        return await GetPagedAsync(pageNumber, pageSize, predicate, orderBy, false);
    }

    public async Task<PagedResult<Role>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Role, bool>> predicate, Expression<Func<Role, object>> orderBy, bool isDescending)
    {
        var query = _dbSet.Where(predicate).Where(r => !r.IsDeleted);
        
        query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Role>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Role> AddAsync(Role entity)
    {
        _dbSet.Add(entity);
        return entity;
    }

    public async Task<IEnumerable<Role>> AddRangeAsync(IEnumerable<Role> entities)
    {
        var entityList = entities.ToList();
        _dbSet.AddRange(entityList);
        return entityList;
    }

    public async Task UpdateAsync(Role entity)
    {
        _dbSet.Update(entity);
    }

    public async Task UpdateRangeAsync(IEnumerable<Role> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public async Task DeleteAsync(Role entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<Role> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task DeleteByIdAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted);
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync()
    {
        return await _dbSet.Where(r => r.IsActive && !r.IsDeleted).ToListAsync();
    }

    public async Task<bool> IsNameExistsAsync(string name)
    {
        return await _dbSet.AnyAsync(r => r.Name == name && !r.IsDeleted);
    }

    public async Task<bool> IsNameExistsAsync(string name, int excludeId)
    {
        return await _dbSet.AnyAsync(r => r.Name == name && r.Id != excludeId && !r.IsDeleted);
    }

    public async Task<PagedResult<Role>> GetPagedActiveRolesAsync(int pageNumber, int pageSize)
    {
        var query = _dbSet.Where(r => r.IsActive && !r.IsDeleted);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Role>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Role>> GetPagedRolesAsync(int pageNumber, int pageSize, Expression<Func<Role, object>> orderBy, bool isDescending = false)
    {
        var query = _dbSet.Where(r => !r.IsDeleted);
        
        query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Role>(items, totalCount, pageNumber, pageSize);
    }
}

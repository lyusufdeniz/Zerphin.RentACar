using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class InsuranceRepository : IInsuranceRepository
{
    private readonly RentACarDbContext _context;

    public InsuranceRepository(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<Insurance?> GetByIdAsync(Guid id)
    {
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<IEnumerable<Insurance>> GetAllAsync()
    {
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> FindAsync(System.Linq.Expressions.Expression<Func<Insurance, bool>> predicate)
    {
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => !i.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Insurance?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Insurance, bool>> predicate)
    {
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => !i.IsDeleted)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Insurance, bool>> predicate)
    {
        return await _context.Insurances
            .Where(i => !i.IsDeleted)
            .AnyAsync(predicate);
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Insurance, bool>>? predicate = null)
    {
        var query = _context.Insurances.Where(i => !i.IsDeleted);
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query.CountAsync();
    }

    public async Task<PagedResult<Insurance>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => !i.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Insurance>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Insurance>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Insurance, bool>> predicate)
    {
        var query = _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => !i.IsDeleted)
            .Where(predicate);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Insurance>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Insurance>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Insurance, bool>> predicate, System.Linq.Expressions.Expression<Func<Insurance, object>> orderBy)
    {
        return await GetPagedAsync(pageNumber, pageSize, predicate, orderBy, false);
    }

    public async Task<PagedResult<Insurance>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Insurance, bool>> predicate, System.Linq.Expressions.Expression<Func<Insurance, object>> orderBy, bool isDescending)
    {
        var query = _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => !i.IsDeleted)
            .Where(predicate);

        query = isDescending 
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Insurance>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Insurance> AddAsync(Insurance entity)
    {
        await _context.Insurances.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<Insurance>> AddRangeAsync(IEnumerable<Insurance> entities)
    {
        await _context.Insurances.AddRangeAsync(entities);
        return entities;
    }

    public async Task UpdateAsync(Insurance entity)
    {
        _context.Insurances.Update(entity);
        await Task.CompletedTask;
    }

    public async Task UpdateRangeAsync(IEnumerable<Insurance> entities)
    {
        _context.Insurances.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Insurance entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Insurances.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<Insurance> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _context.Insurances.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    public async Task<Insurance?> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => i.VehicleId == vehicleId && !i.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<Insurance?> GetByPolicyNumberAsync(string policyNumber)
    {
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => i.PolicyNumber == policyNumber && !i.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Insurance>> GetActiveInsurancesAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => i.IsActive && i.StartDate <= now && i.EndDate >= now && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> GetExpiredInsurancesAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => i.EndDate < now && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Insurance>> GetExpiringSoonAsync(int days)
    {
        var now = DateTime.UtcNow;
        var expiryDate = now.AddDays(days);
        return await _context.Insurances
            .Include(i => i.Vehicle)
            .Where(i => i.EndDate >= now && i.EndDate <= expiryDate && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> IsPolicyNumberExistsAsync(string policyNumber)
    {
        return await _context.Insurances
            .AnyAsync(i => i.PolicyNumber == policyNumber && !i.IsDeleted);
    }
}


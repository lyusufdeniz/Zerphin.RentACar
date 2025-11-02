using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly RentACarDbContext _context;

    public RentalRepository(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> FindAsync(System.Linq.Expressions.Expression<Func<Rental, bool>> predicate)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => !r.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Rental?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Rental, bool>> predicate)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Rental, bool>> predicate)
    {
        return await _context.Rentals
            .Where(r => !r.IsDeleted)
            .AnyAsync(predicate);
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Rental, bool>>? predicate = null)
    {
        var query = _context.Rentals.Where(r => !r.IsDeleted);
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query.CountAsync();
    }

    public async Task<PagedResult<Rental>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => !r.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Rental>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Rental, bool>> predicate)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => !r.IsDeleted)
            .Where(predicate);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Rental>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Rental, bool>> predicate, System.Linq.Expressions.Expression<Func<Rental, object>> orderBy)
    {
        return await GetPagedAsync(pageNumber, pageSize, predicate, orderBy, false);
    }

    public async Task<PagedResult<Rental>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Rental, bool>> predicate, System.Linq.Expressions.Expression<Func<Rental, object>> orderBy, bool isDescending)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => !r.IsDeleted)
            .Where(predicate);

        query = isDescending 
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Rental> AddAsync(Rental entity)
    {
        await _context.Rentals.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<Rental>> AddRangeAsync(IEnumerable<Rental> entities)
    {
        await _context.Rentals.AddRangeAsync(entities);
        return entities;
    }

    public async Task UpdateAsync(Rental entity)
    {
        _context.Rentals.Update(entity);
        await Task.CompletedTask;
    }

    public async Task UpdateRangeAsync(IEnumerable<Rental> entities)
    {
        _context.Rentals.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Rental entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Rentals.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<Rental> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _context.Rentals.UpdateRange(entities);
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

    public async Task<IEnumerable<Rental>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.CustomerId == customerId && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.VehicleId == vehicleId && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == status && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.StartDate >= startDate && r.EndDate <= endDate && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetOverdueRentalsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active && r.EndDate < now && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<Rental?> GetActiveRentalByVehicleIdAsync(Guid vehicleId)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.VehicleId == vehicleId && r.Status == RentalStatus.Active && !r.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResult<Rental>> GetPagedByCustomerIdAsync(Guid customerId, int pageNumber, int pageSize)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.CustomerId == customerId && !r.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Rental>> GetPagedByVehicleIdAsync(Guid vehicleId, int pageNumber, int pageSize)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.VehicleId == vehicleId && !r.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Rental>> GetPagedByStatusAsync(RentalStatus status, int pageNumber, int pageSize)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == status && !r.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Rental>> GetPagedActiveRentalsAsync(int pageNumber, int pageSize)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active && !r.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Rental>> GetPagedByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.StartDate >= startDate && r.EndDate <= endDate && !r.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Rental>> GetPagedOverdueRentalsAsync(int pageNumber, int pageSize)
    {
        var now = DateTime.UtcNow;
        var query = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active && r.EndDate < now && !r.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Rental>(items, totalCount, pageNumber, pageSize);
    }
}


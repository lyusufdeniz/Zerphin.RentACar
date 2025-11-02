using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly RentACarDbContext _context;

    public VehicleRepository(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles
            .Where(v => !v.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> FindAsync(System.Linq.Expressions.Expression<Func<Vehicle, bool>> predicate)
    {
        return await _context.Vehicles
            .Where(v => !v.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Vehicle?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Vehicle, bool>> predicate)
    {
        return await _context.Vehicles
            .Where(v => !v.IsDeleted)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Vehicle, bool>> predicate)
    {
        return await _context.Vehicles
            .Where(v => !v.IsDeleted)
            .AnyAsync(predicate);
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Vehicle, bool>>? predicate = null)
    {
        var query = _context.Vehicles.Where(v => !v.IsDeleted);
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return await query.CountAsync();
    }

    public async Task<PagedResult<Vehicle>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Vehicles
            .Where(v => !v.IsDeleted);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Vehicle>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Vehicle, bool>> predicate)
    {
        var query = _context.Vehicles
            .Where(v => !v.IsDeleted)
            .Where(predicate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Vehicle>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Vehicle, bool>> predicate, System.Linq.Expressions.Expression<Func<Vehicle, object>> orderBy)
    {
        var query = _context.Vehicles
            .Where(v => !v.IsDeleted)
            .Where(predicate)
            .OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Vehicle>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Vehicle, bool>> predicate, System.Linq.Expressions.Expression<Func<Vehicle, object>> orderBy, bool isDescending)
    {
        var query = _context.Vehicles
            .Where(v => !v.IsDeleted)
            .Where(predicate);

        query = isDescending 
            ? query.OrderByDescending(orderBy) 
            : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Vehicle> AddAsync(Vehicle entity)
    {
        _context.Vehicles.Add(entity);
        return entity;
    }

    public async Task<IEnumerable<Vehicle>> AddRangeAsync(IEnumerable<Vehicle> entities)
    {
        var entityList = entities.ToList();
        _context.Vehicles.AddRange(entityList);
        return entityList;
    }

    public async Task UpdateAsync(Vehicle entity)
    {
        _context.Vehicles.Update(entity);
    }

    public async Task UpdateRangeAsync(IEnumerable<Vehicle> entities)
    {
        var entityList = entities.ToList();
        _context.Vehicles.UpdateRange(entityList);
    }

    public async Task DeleteAsync(Vehicle entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Vehicles.Update(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<Vehicle> entities)
    {
        var entityList = entities.ToList();
        foreach (var entity in entityList)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _context.Vehicles.UpdateRange(entityList);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            vehicle.IsDeleted = true;
            vehicle.DeletedAt = DateTime.UtcNow;
            _context.Vehicles.Update(vehicle);
        }
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate && !v.IsDeleted);
    }

    public async Task<IEnumerable<Vehicle>> GetByCategoryAsync(VehicleCategory category)
    {
        return await _context.Vehicles
            .Where(v => v.Category == category && !v.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status)
    {
        return await _context.Vehicles
            .Where(v => v.Status == status && !v.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Available && !v.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Vehicles
            .Where(v => v.DailyRentalPrice >= minPrice && v.DailyRentalPrice <= maxPrice && !v.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> IsLicensePlateExistsAsync(string licensePlate)
    {
        return await _context.Vehicles
            .AnyAsync(v => v.LicensePlate == licensePlate && !v.IsDeleted);
    }

    public async Task<PagedResult<Vehicle>> GetPagedByCategoryAsync(VehicleCategory category, int pageNumber, int pageSize)
    {
        var query = _context.Vehicles
            .Where(v => v.Category == category && !v.IsDeleted);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Vehicle>> GetPagedByStatusAsync(VehicleStatus status, int pageNumber, int pageSize)
    {
        var query = _context.Vehicles
            .Where(v => v.Status == status && !v.IsDeleted);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Vehicle>> GetPagedAvailableVehiclesAsync(int pageNumber, int pageSize)
    {
        var query = _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Available && !v.IsDeleted);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Vehicle>> GetPagedByPriceRangeAsync(decimal minPrice, decimal maxPrice, int pageNumber, int pageSize)
    {
        var query = _context.Vehicles
            .Where(v => v.DailyRentalPrice >= minPrice && v.DailyRentalPrice <= maxPrice && !v.IsDeleted);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }
}



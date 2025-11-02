using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly RentACarDbContext _context;

    public CustomerRepository(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.User)
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> FindAsync(System.Linq.Expressions.Expression<Func<Customer, bool>> predicate)
    {
        return await _context.Customers
            .Include(c => c.User)
            .Where(c => !c.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Customer?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Customer, bool>> predicate)
    {
        return await _context.Customers
            .Include(c => c.User)
            .Where(c => !c.IsDeleted)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Customer, bool>> predicate)
    {
        return await _context.Customers
            .Where(c => !c.IsDeleted)
            .AnyAsync(predicate);
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Customer, bool>>? predicate = null)
    {
        var query = _context.Customers.Where(c => !c.IsDeleted);
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query.CountAsync();
    }

    public async Task<Domain.Common.PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Customers
            .Include(c => c.User)
            .Where(c => !c.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Domain.Common.PagedResult<Customer>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Domain.Common.PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Customer, bool>> predicate)
    {
        var query = _context.Customers
            .Include(c => c.User)
            .Where(c => !c.IsDeleted)
            .Where(predicate);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Domain.Common.PagedResult<Customer>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Domain.Common.PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Customer, bool>> predicate, System.Linq.Expressions.Expression<Func<Customer, object>> orderBy)
    {
        return await GetPagedAsync(pageNumber, pageSize, predicate, orderBy, false);
    }

    public async Task<Domain.Common.PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Customer, bool>> predicate, System.Linq.Expressions.Expression<Func<Customer, object>> orderBy, bool isDescending)
    {
        var query = _context.Customers
            .Include(c => c.User)
            .Where(c => !c.IsDeleted)
            .Where(predicate);

        query = isDescending 
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Domain.Common.PagedResult<Customer>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Customer> AddAsync(Customer entity)
    {
        await _context.Customers.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<Customer>> AddRangeAsync(IEnumerable<Customer> entities)
    {
        await _context.Customers.AddRangeAsync(entities);
        return entities;
    }

    public async Task UpdateAsync(Customer entity)
    {
        _context.Customers.Update(entity);
        await Task.CompletedTask;
    }

    public async Task UpdateRangeAsync(IEnumerable<Customer> entities)
    {
        _context.Customers.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Customer entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Customers.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<Customer> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _context.Customers.UpdateRange(entities);
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

    public async Task<Customer?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
    }

    public async Task<Customer?> GetByLicenseNumberAsync(string licenseNumber)
    {
        return await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.LicenseNumber == licenseNumber && !c.IsDeleted);
    }

    public async Task<IEnumerable<Customer>> GetVerifiedCustomersAsync()
    {
        return await _context.Customers
            .Include(c => c.User)
            .Where(c => c.IsVerified && !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetByCreditScoreRangeAsync(int minScore, int maxScore)
    {
        return await _context.Customers
            .Include(c => c.User)
            .Where(c => c.CreditScore >= minScore && c.CreditScore <= maxScore && !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> IsLicenseNumberExistsAsync(string licenseNumber)
    {
        return await _context.Customers
            .AnyAsync(c => c.LicenseNumber == licenseNumber && !c.IsDeleted);
    }
}


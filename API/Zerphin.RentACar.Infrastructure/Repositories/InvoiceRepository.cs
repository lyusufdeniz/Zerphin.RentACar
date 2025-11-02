using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly RentACarDbContext _context;

    public InvoiceRepository(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> FindAsync(System.Linq.Expressions.Expression<Func<Invoice, bool>> predicate)
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => !i.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Invoice?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Invoice, bool>> predicate)
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => !i.IsDeleted)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Invoice, bool>> predicate)
    {
        return await _context.Invoices
            .Where(i => !i.IsDeleted)
            .AnyAsync(predicate);
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Invoice, bool>>? predicate = null)
    {
        var query = _context.Invoices.Where(i => !i.IsDeleted);
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query.CountAsync();
    }

    public async Task<PagedResult<Invoice>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => !i.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Invoice>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Invoice>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Invoice, bool>> predicate)
    {
        var query = _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => !i.IsDeleted)
            .Where(predicate);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Invoice>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Invoice>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Invoice, bool>> predicate, System.Linq.Expressions.Expression<Func<Invoice, object>> orderBy)
    {
        return await GetPagedAsync(pageNumber, pageSize, predicate, orderBy, false);
    }

    public async Task<PagedResult<Invoice>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Invoice, bool>> predicate, System.Linq.Expressions.Expression<Func<Invoice, object>> orderBy, bool isDescending)
    {
        var query = _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
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

        return new PagedResult<Invoice>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Invoice> AddAsync(Invoice entity)
    {
        await _context.Invoices.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<Invoice>> AddRangeAsync(IEnumerable<Invoice> entities)
    {
        await _context.Invoices.AddRangeAsync(entities);
        return entities;
    }

    public async Task UpdateAsync(Invoice entity)
    {
        _context.Invoices.Update(entity);
        await Task.CompletedTask;
    }

    public async Task UpdateRangeAsync(IEnumerable<Invoice> entities)
    {
        _context.Invoices.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Invoice entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Invoices.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<Invoice> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _context.Invoices.UpdateRange(entities);
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

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => i.InvoiceNumber == invoiceNumber && !i.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<Invoice?> GetByRentalIdAsync(Guid rentalId)
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => i.RentalId == rentalId && !i.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Invoice>> GetPaidInvoicesAsync()
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => i.IsPaid && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => !i.IsPaid && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Invoices
            .Include(i => i.Rental)
                .ThenInclude(r => r.Customer)
            .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> IsInvoiceNumberExistsAsync(string invoiceNumber)
    {
        return await _context.Invoices
            .AnyAsync(i => i.InvoiceNumber == invoiceNumber && !i.IsDeleted);
    }
}


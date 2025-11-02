using Microsoft.EntityFrameworkCore;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly RentACarDbContext _context;

    public PaymentRepository(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> FindAsync(System.Linq.Expressions.Expression<Func<Payment, bool>> predicate)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => !p.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Payment?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Payment, bool>> predicate)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => !p.IsDeleted)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Payment, bool>> predicate)
    {
        return await _context.Payments
            .Where(p => !p.IsDeleted)
            .AnyAsync(predicate);
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Payment, bool>>? predicate = null)
    {
        var query = _context.Payments.Where(p => !p.IsDeleted);
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query.CountAsync();
    }

    public async Task<PagedResult<Payment>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => !p.IsDeleted);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Payment>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Payment>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Payment, bool>> predicate)
    {
        var query = _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => !p.IsDeleted)
            .Where(predicate);
            
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Payment>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<Payment>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Payment, bool>> predicate, System.Linq.Expressions.Expression<Func<Payment, object>> orderBy)
    {
        return await GetPagedAsync(pageNumber, pageSize, predicate, orderBy, false);
    }

    public async Task<PagedResult<Payment>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<Payment, bool>> predicate, System.Linq.Expressions.Expression<Func<Payment, object>> orderBy, bool isDescending)
    {
        var query = _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => !p.IsDeleted)
            .Where(predicate);

        query = isDescending 
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Payment>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Payment> AddAsync(Payment entity)
    {
        await _context.Payments.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<Payment>> AddRangeAsync(IEnumerable<Payment> entities)
    {
        await _context.Payments.AddRangeAsync(entities);
        return entities;
    }

    public async Task UpdateAsync(Payment entity)
    {
        _context.Payments.Update(entity);
        await Task.CompletedTask;
    }

    public async Task UpdateRangeAsync(IEnumerable<Payment> entities)
    {
        _context.Payments.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Payment entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Payments.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<Payment> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _context.Payments.UpdateRange(entities);
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

    public async Task<IEnumerable<Payment>> GetByRentalIdAsync(Guid rentalId)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => p.RentalId == rentalId && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => p.Status == status && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByMethodAsync(PaymentMethod method)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => p.Method == method && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
    {
        return await _context.Payments
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
            .Where(p => p.TransactionId == transactionId && !p.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<decimal> GetTotalAmountByRentalIdAsync(Guid rentalId)
    {
        return await _context.Payments
            .Where(p => p.RentalId == rentalId && !p.IsDeleted)
            .SumAsync(p => p.Amount);
    }
}


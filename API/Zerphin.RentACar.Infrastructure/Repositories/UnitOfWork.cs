using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Infrastructure.Data;

namespace Zerphin.RentACar.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly RentACarDbContext _context;

    public UnitOfWork(RentACarDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

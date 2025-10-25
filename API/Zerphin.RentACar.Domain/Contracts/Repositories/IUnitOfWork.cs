namespace Zerphin.RentACar.Domain.Contracts.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    int SaveChanges();
}

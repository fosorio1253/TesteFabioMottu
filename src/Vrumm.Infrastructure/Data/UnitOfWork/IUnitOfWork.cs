using Vrumm.Domain.Repositories;

namespace Vrumm.Infrastructure.Data.UnitOfWork;
public interface IUnitOfWork : IDisposable
{
    IMotorcycleRepository Motorcycles { get; }
    IDriverRepository Drivers { get; }
    IPlanRepository Plans { get; }
    IRentalRepository Rentals { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
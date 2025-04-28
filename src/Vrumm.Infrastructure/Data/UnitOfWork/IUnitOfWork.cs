using Vrumm.Domain.Repositories;

namespace Vrumm.Infrastructure.Data.UnitOfWork;
public interface IUnitOfWork : IDisposable
{
    IMotorcycleRepository Motorcycles { get; }
    IDriverRepository Drivers { get; }
    IPlanRepository Plans { get; }
    IRentalRepository Rentals { get; }
    IMotorcycleRegistrationEventRepository MotorcycleRegistrationEvents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
}
using Microsoft.EntityFrameworkCore.Storage;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly VrummDbContext _context;
    private IDbContextTransaction _transaction;
    private bool _disposed = false;

    public IMotorcycleRepository Motorcycles { get; }
    public IDriverRepository Drivers { get; }
    public IPlanRepository Plans { get; }
    public IRentalRepository Rentals { get; }

    public UnitOfWork(
        VrummDbContext context,
        IMotorcycleRepository motorcycleRepository,
        IDriverRepository driverRepository,
        IPlanRepository planRepository,
        IRentalRepository rentalRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Motorcycles = motorcycleRepository ?? throw new ArgumentNullException(nameof(motorcycleRepository));
        Drivers = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        Plans = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
        Rentals = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _transaction?.CommitAsync(cancellationToken);
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _transaction?.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}
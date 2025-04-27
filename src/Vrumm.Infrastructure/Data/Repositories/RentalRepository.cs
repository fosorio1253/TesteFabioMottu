using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class RentalRepository : Repository<Rental, Guid>, IRentalRepository
{
    public RentalRepository(VrummDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rental>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(r => r.DriverId == driverId).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(r => r.MotorcycleId == motorcycleId).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.Where(r => r.Status == RentalStatus.Active).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(r => r.Status == status).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(r => r.StartDate >= startDate && r.StartDate <= endDate).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> HasActiveRentalForMotorcycleAsync(Guid motorcycleId, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(r => r.MotorcycleId == motorcycleId && r.Status == RentalStatus.Active, cancellationToken: cancellationToken);
    }

    public override async Task<Rental> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(r => r.MotorcycleId)
            .Include(r => r.DriverId)
            .Include(r => r.PlanId)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);
    }

    public IQueryable<Rental> GetAll()
    {
        return _dbSet.AsQueryable<Rental>();
    }
}
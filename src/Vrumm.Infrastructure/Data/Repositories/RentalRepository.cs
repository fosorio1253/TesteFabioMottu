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

    public async Task<IEnumerable<Rental>> GetByDriverIdAsync(Guid driverId)
    {
        return await _dbSet.Where(r => r.DriverId == driverId).ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId)
    {
        return await _dbSet.Where(r => r.MotorcycleId == motorcycleId).ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        return await _dbSet.Where(r => r.Status == RentalStatus.Active).ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status)
    {
        return await _dbSet.Where(r => r.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet.Where(r => r.StartDate >= startDate && r.StartDate <= endDate).ToListAsync();
    }

    public async Task<bool> HasActiveRentalForMotorcycleAsync(Guid motorcycleId)
    {
        return await _dbSet.AnyAsync(r => r.MotorcycleId == motorcycleId && r.Status == RentalStatus.Active);
    }

    public override async Task<Rental> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.MotorcycleId)
            .Include(r => r.DriverId)
            .Include(r => r.PlanId)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
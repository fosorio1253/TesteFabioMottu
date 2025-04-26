using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class MotorcycleRepository : Repository<Motorcycle, Guid>, IMotorcycleRepository
{
    public MotorcycleRepository(VrummDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByLicensePlateAsync(string licensePlate)
    {
        return await _dbSet.AnyAsync(m => m.LicensePlate == licensePlate);
    }

    public async Task<bool> ExistsByLicensePlateExceptIdAsync(string licensePlate, Guid id)
    {
        return await _dbSet.AnyAsync(m => m.LicensePlate == licensePlate && m.Id != id);
    }

    public async Task<IEnumerable<Motorcycle>> GetByStatusAsync(MotorcycleStatus status)
    {
        return await _dbSet.Where(m => m.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Motorcycle>> GetByYearRangeAsync(int startYear, int endYear)
    {
        return await _dbSet.Where(m => m.Year >= startYear && m.Year <= endYear).ToListAsync();
    }
}
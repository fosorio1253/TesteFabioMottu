using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class MotorcycleRepository : Repository<Motorcycle, Guid>, IMotorcycleRepository
{
    public MotorcycleRepository(VrummDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(m => EF.Property<string>(m, "LicensePlate") == licensePlate,
                                    cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByLicensePlateExceptIdAsync(LicensePlate licensePlate, Guid id,
                                                            CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(m => EF.Property<string>(m, "LicensePlate") == licensePlate.ToStringRepresentation()
                                       && m.Id != id,
                                    cancellationToken: cancellationToken);
    }

    public IQueryable<Motorcycle> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<IEnumerable<Motorcycle>> GetByStatusAsync(MotorcycleStatus status,
                                                              CancellationToken cancellationToken)
    {
        return await _dbSet.Where(m => EF.Property<MotorcycleStatus>(m, "Status") == status)
                          .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Motorcycle>> GetByYearRangeAsync(int startYear, int endYear,
                                                                 CancellationToken cancellationToken)
    {
        return await _dbSet.Where(m => EF.Property<int>(m, "Year") >= startYear
                                    && EF.Property<int>(m, "Year") <= endYear)
                          .ToListAsync(cancellationToken: cancellationToken);
    }
}
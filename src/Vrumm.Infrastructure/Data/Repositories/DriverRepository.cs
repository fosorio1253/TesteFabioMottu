using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class DriverRepository : Repository<Driver, Guid>, IDriverRepository
{
    public DriverRepository(VrummDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.TaxId == taxId, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByTaxIdExceptIdAsync(string taxId, Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.TaxId == taxId && d.Id != id, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByLicenseNumberExceptIdAsync(string licenseNumber, Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber && d.Id != id, cancellationToken: cancellationToken);
    }

    public async Task<Driver> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.TaxId == taxId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Driver>> GetByLicenseTypeAsync(LicenseType licenseType, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(d => d.LicenseType == licenseType || d.LicenseType == LicenseType.AB).ToListAsync(cancellationToken: cancellationToken);
    }

    public IQueryable<Driver> GetAll()
    {
        return _dbSet.AsQueryable();
    }
}
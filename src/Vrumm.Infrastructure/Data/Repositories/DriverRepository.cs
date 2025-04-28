using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class DriverRepository : Repository<Driver, Guid>, IDriverRepository
{
    public DriverRepository(VrummDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.Cnpj == cnpj, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByLicenseNumberAsync(LicenseNumber licenseNumber, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByCnpjExceptIdAsync(Cnpj cnpj, Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.Cnpj == cnpj && d.Id != id, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByLicenseNumberExceptIdAsync(LicenseNumber licenseNumber, Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber && d.Id != id, cancellationToken: cancellationToken);
    }

    public async Task<Driver> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Cnpj == cnpj, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Driver>> GetByLicenseTypeAsync(LicenseTypeValue licenseType, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(d => d.LicenseType == licenseType || d.LicenseType.Value == LicenseType.AB).ToListAsync(cancellationToken: cancellationToken);
    }

    public IQueryable<Driver> GetAll()
    {
        return _dbSet.AsQueryable();
    }
}
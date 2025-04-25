using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Commom.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class DriverRepository : Repository<Driver, Guid>, IDriverRepository
{
    public DriverRepository(VrummDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByTaxIdAsync(string taxId)
    {
        return await _dbSet.AnyAsync(d => d.TaxId == taxId);
    }

    public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
    {
        return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber);
    }

    public async Task<bool> ExistsByTaxIdExceptIdAsync(string taxId, Guid id)
    {
        return await _dbSet.AnyAsync(d => d.TaxId == taxId && d.Id != id);
    }

    public async Task<bool> ExistsByLicenseNumberExceptIdAsync(string licenseNumber, Guid id)
    {
        return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber && d.Id != id);
    }

    public async Task<Driver> GetByTaxIdAsync(string taxId)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.TaxId == taxId);
    }

    public async Task<IEnumerable<Driver>> GetByLicenseTypeAsync(LicenseType licenseType)
    {
        return await _dbSet.Where(d => d.LicenseType == licenseType || d.LicenseType == LicenseType.AB).ToListAsync();
    }
}
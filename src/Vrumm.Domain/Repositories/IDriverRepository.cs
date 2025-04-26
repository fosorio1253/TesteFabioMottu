using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;

namespace Vrumm.Domain.Repositories;
public interface IDriverRepository : IRepository<Driver, Guid>
{
    Task<bool> ExistsByTaxIdAsync(string taxId);
    Task<bool> ExistsByLicenseNumberAsync(string licenseNumber);
    Task<bool> ExistsByTaxIdExceptIdAsync(string taxId, Guid id);
    Task<bool> ExistsByLicenseNumberExceptIdAsync(string licenseNumber, Guid id);
    Task<Driver> GetByTaxIdAsync(string taxId);
    Task<IEnumerable<Driver>> GetByLicenseTypeAsync(LicenseType licenseType);
}
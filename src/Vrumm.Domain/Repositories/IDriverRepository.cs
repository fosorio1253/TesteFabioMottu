using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;

namespace Vrumm.Domain.Repositories;
public interface IDriverRepository : IRepository<Driver, Guid>
{
    Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken);
    Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken);
    Task<bool> ExistsByTaxIdExceptIdAsync(string taxId, Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByLicenseNumberExceptIdAsync(string licenseNumber, Guid id, CancellationToken cancellationToken);
    Task<Driver> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken);
    Task<IEnumerable<Driver>> GetByLicenseTypeAsync(LicenseType licenseType, CancellationToken cancellationToken);
    IQueryable<Driver> GetAll();
}
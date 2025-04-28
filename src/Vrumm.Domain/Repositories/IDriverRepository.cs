using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;

namespace Vrumm.Domain.Repositories;
public interface IDriverRepository : IRepository<Driver, Guid>
{
    Task<bool> ExistsByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken);
    Task<bool> ExistsByLicenseNumberAsync(LicenseNumber licenseNumber, CancellationToken cancellationToken);
    Task<bool> ExistsByCnpjExceptIdAsync(Cnpj cnpj, Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByLicenseNumberExceptIdAsync(LicenseNumber licenseNumber, Guid id, CancellationToken cancellationToken);
    Task<Driver> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken);
    Task<IEnumerable<Driver>> GetByLicenseTypeAsync(LicenseTypeValue licenseType, CancellationToken cancellationToken);
    IQueryable<Driver> GetAll();
}
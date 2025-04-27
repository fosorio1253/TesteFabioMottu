using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;

namespace Vrumm.Domain.Repositories;
public interface IMotorcycleRepository : IRepository<Motorcycle, Guid>
{
    Task<bool> ExistsByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken);
    Task<bool> ExistsByLicensePlateExceptIdAsync(string licensePlate, Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Motorcycle>> GetByStatusAsync(MotorcycleStatus status, CancellationToken cancellationToken);
    Task<IEnumerable<Motorcycle>> GetByYearRangeAsync(int startYear, int endYear, CancellationToken cancellationToken);
    IQueryable<Motorcycle> GetAll();
}
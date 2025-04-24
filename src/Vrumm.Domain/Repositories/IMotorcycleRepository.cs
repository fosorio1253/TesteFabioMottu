using Vrumm.Domain.Commom.Enums;
using Vrumm.Domain.Entities;

namespace Vrumm.Domain.Repositories;
public interface IMotorcycleRepository : IRepository<Motorcycle, Guid>
{
    Task<bool> ExistsByLicensePlateAsync(string licensePlate);
    Task<bool> ExistsByLicensePlateExceptIdAsync(string licensePlate, Guid id);
    Task<IEnumerable<Motorcycle>> GetByStatusAsync(MotorcycleStatus status);
    Task<IEnumerable<Motorcycle>> GetByYearRangeAsync(int startYear, int endYear);
}
using Vrumm.Domain.Commom.Enums;
using Vrumm.Domain.Entities;

namespace Vrumm.Domain.Repositories;
public interface IRentalRepository : IRepository<Rental, Guid>
{
    Task<IEnumerable<Rental>> GetByDriverIdAsync(Guid driverId);
    Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId);
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status);
    Task<IEnumerable<Rental>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> HasActiveRentalForMotorcycleAsync(Guid motorcycleId);
}
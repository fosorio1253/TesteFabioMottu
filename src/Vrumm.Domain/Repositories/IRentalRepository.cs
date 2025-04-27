using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;

namespace Vrumm.Domain.Repositories;
public interface IRentalRepository : IRepository<Rental, Guid>
{
    Task<IEnumerable<Rental>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken);
    Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken);
    Task<IEnumerable<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status, CancellationToken cancellationToken);
    Task<IEnumerable<Rental>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    Task<bool> HasActiveRentalForMotorcycleAsync(Guid motorcycleId, CancellationToken cancellationToken);
    IQueryable<Rental> GetAll();
}
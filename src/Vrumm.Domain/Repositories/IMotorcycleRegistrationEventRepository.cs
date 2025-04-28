using Vrumm.Domain.Entities.MotorcycleCompose;

namespace Vrumm.Domain.Repositories;
public interface IMotorcycleRegistrationEventRepository
{
    Task<bool> ExistsByMotorcycleIdAsync(Guid motorcycleId, DateTime eventTimestamp, CancellationToken cancellationToken = default);
    Task AddAsync(MotorcycleRegistrationEvent registrationEvent, CancellationToken cancellationToken = default);
    Task<IEnumerable<MotorcycleRegistrationEvent>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MotorcycleRegistrationEvent>> GetByYearAsync(int year, CancellationToken cancellationToken = default);
}
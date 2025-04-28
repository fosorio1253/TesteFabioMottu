using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class MotorcycleRegistrationEventRepository : IMotorcycleRegistrationEventRepository
{
    private readonly VrummDbContext _context;

    public MotorcycleRegistrationEventRepository(VrummDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> ExistsByMotorcycleIdAsync(Guid motorcycleId, DateTime eventTimestamp, CancellationToken cancellationToken = default)
    {
        return await _context.Set<MotorcycleRegistrationEvent>()
            .AnyAsync(e => e.MotorcycleId == motorcycleId && e.EventTimestamp == eventTimestamp, cancellationToken);
    }

    public async Task AddAsync(MotorcycleRegistrationEvent registrationEvent, CancellationToken cancellationToken = default)
    {
        await _context.Set<MotorcycleRegistrationEvent>().AddAsync(registrationEvent, cancellationToken);
    }

    public async Task<IEnumerable<MotorcycleRegistrationEvent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<MotorcycleRegistrationEvent>()
            .OrderByDescending(e => e.EventTimestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MotorcycleRegistrationEvent>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        return await _context.Set<MotorcycleRegistrationEvent>()
            .Where(e => e.Year == year)
            .OrderByDescending(e => e.EventTimestamp)
            .ToListAsync(cancellationToken);
    }
}

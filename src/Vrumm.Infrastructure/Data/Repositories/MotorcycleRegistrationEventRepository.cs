using System.Linq.Expressions;
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

    public Task<MotorcycleRegistrationEvent> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MotorcycleRegistrationEvent>> FindAsync(Expression<Func<MotorcycleRegistrationEvent, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Expression<Func<MotorcycleRegistrationEvent, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(MotorcycleRegistrationEvent entity)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(MotorcycleRegistrationEvent entity)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<MotorcycleRegistrationEvent>> GetQueryAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

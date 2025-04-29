using Microsoft.EntityFrameworkCore;
using Vrumm.Application.Plans;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class PlanRepository : Repository<Plan, int>, IPlanRepository
{
    private IPlanFactory _planFactory;
    public PlanRepository(VrummDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Plan>> GetByDayCountRangeAsync(int minDays, int maxDays, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(p => p.DayCount >= minDays && p.DayCount <= maxDays)
            .ToListAsync(cancellationToken);
    }

    public async Task SeedDefaultPlansAsync(CancellationToken cancellationToken)
    {
        if (!await _dbSet.AnyAsync(cancellationToken))
        {
            var plans = Enumerable.Range(1, 5).Select(id => _planFactory.CreatePlan(id));
            await _dbSet.AddRangeAsync(plans, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
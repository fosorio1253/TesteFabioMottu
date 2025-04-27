using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class PlanRepository : Repository<Plan, int>, IPlanRepository
{
    public PlanRepository(VrummDbContext context) : base(context)
    {
    }

    public IQueryable<Plan> GetAll()
    {
        return _dbSet.AsQueryable<Plan>();
    }

    public async Task<IEnumerable<Plan>> GetByDayCountRangeAsync(int minDays, int maxDays, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(p => p.DayCount >= minDays && p.DayCount <= maxDays).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task SeedDefaultPlansAsync(CancellationToken cancellationToken)
    {
        if (!await _dbSet.AnyAsync(cancellationToken: cancellationToken))
        {
            await _dbSet.AddRangeAsync(
                Plan.CreateSevenDayPlan(),
                Plan.CreateFifteenDayPlan(),
                Plan.CreateThirtyDayPlan(),
                Plan.CreateFortyFiveDayPlan(),
                Plan.CreateFiftyDayPlan()
            );

            await _context.SaveChangesAsync();
        }
    }
}
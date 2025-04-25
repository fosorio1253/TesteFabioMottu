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

    public async Task<IEnumerable<Plan>> GetByDayCountRangeAsync(int minDays, int maxDays)
    {
        return await _dbSet.Where(p => p.DayCount >= minDays && p.DayCount <= maxDays).ToListAsync();
    }

    public async Task SeedDefaultPlansAsync()
    {
        if (!await _dbSet.AnyAsync())
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
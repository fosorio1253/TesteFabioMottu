using Vrumm.Domain.Entities;

namespace Vrumm.Domain.Repositories;
public interface IPlanRepository : IRepository<Plan, int>
{
    Task<IEnumerable<Plan>> GetByDayCountRangeAsync(int minDays, int maxDays);
    Task SeedDefaultPlansAsync();
}
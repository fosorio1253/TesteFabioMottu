using Vrumm.Domain.Entities.PlanCompose;

namespace Vrumm.Domain.Repositories;
public interface IPlanRepository : IRepository<Plan, int>
{
    Task<IEnumerable<Plan>> GetByDayCountRangeAsync(int minDays, int maxDays, CancellationToken cancellationToken);
    Task SeedDefaultPlansAsync(CancellationToken cancellationToken);
}
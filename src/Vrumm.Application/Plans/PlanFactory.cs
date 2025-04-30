using Microsoft.Extensions.Options;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Options;

namespace Vrumm.Application.Plans;
public class PlanFactory : IPlanFactory
{
    private readonly PlanOptions _planOptions;

    public PlanFactory(IOptions<PlanOptions> planOptions)
    {
        _planOptions = planOptions.Value ?? throw new ArgumentNullException(nameof(planOptions));
    }

    public Plan CreatePlan(int planId)
    {
        var planConfig = _planOptions.Configurations.ElementAtOrDefault(planId - 1);
        if (planConfig == null)
            throw new ArgumentException($"Plan with ID {planId} not found.");

        return new Plan(planId, planConfig.DayCount, planConfig.DailyRate,
            planConfig.PenaltyPercentage, planConfig.AdditionalDayRate);
    }
}
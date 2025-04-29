using Microsoft.Extensions.Configuration;
using Vrumm.Domain.Entities;

namespace Vrumm.Application.Plans;
public class PlanFactory : IPlanFactory
{
    private readonly IConfiguration _configuration;

    public PlanFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Plan CreatePlan(int planId)
    {
        var plans = _configuration.GetSection("Plans").Get<List<PlanConfig>>();
        if (plans == null || !plans.Any())
        {
            throw new InvalidOperationException("No plans configured in appsettings.json.");
        }

        var planConfig = plans.ElementAtOrDefault(planId - 1);
        if (planConfig == null)
        {
            throw new ArgumentException($"Plan with ID {planId} not found.");
        }

        return new Plan(planId, planConfig.DayCount, planConfig.DailyRate,
            planConfig.PenaltyPercentage, planConfig.AdditionalDayRate);
    }
}
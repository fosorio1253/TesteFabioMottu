using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Vrumm.Infrastructure.Data.Configuration;
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    private readonly IConfiguration _configuration;

    public PlanConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.DayCount).IsRequired();
        builder.Property(p => p.DailyRate).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.PenaltyPercentage).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.AdditionalDayRate).HasPrecision(18, 2).IsRequired();

        var plans = _configuration.GetSection("Plans").Get<List<PlanConfig>>();
        if (plans == null || !plans.Any())
        {
            throw new InvalidOperationException("No plans configured in appsettings.json.");
        }

        builder.HasData(plans.Select((p, index) => new Plan(
            index + 1,
            p.DayCount,
            p.DailyRate,
            p.PenaltyPercentage,
            p.AdditionalDayRate
        )));
    }

    private class PlanConfig
    {
        public int DayCount { get; set; }
        public decimal DailyRate { get; set; }
        public decimal PenaltyPercentage { get; set; }
        public decimal AdditionalDayRate { get; set; }
    }
}
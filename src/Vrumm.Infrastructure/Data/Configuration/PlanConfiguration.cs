using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Options;

namespace Vrumm.Infrastructure.Data.Configuration;
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    private readonly PlanOptions _planOptions;

    public PlanConfiguration(IOptions<PlanOptions> planOptions)
    {
        _planOptions = planOptions.Value ?? throw new ArgumentNullException(nameof(planOptions));
    }

    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.DayCount).IsRequired();
        builder.Property(p => p.DailyRate).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.PenaltyPercentage).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.AdditionalDayRate).HasPrecision(18, 2).IsRequired();

        if (_planOptions.Configurations == null || !_planOptions.Configurations.Any())
        {
            throw new InvalidOperationException("No plans configured in appsettings.json.");
        }

        builder.HasData(_planOptions.Configurations.Select((p, index) => new Plan(
            index + 1,
            p.DayCount,
            p.DailyRate,
            p.PenaltyPercentage,
            p.AdditionalDayRate
        )));
    }
}
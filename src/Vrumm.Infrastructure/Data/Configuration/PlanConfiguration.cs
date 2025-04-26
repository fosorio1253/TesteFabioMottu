using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities;

namespace Vrumm.Infrastructure.Data.Configuration;
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.DayCount)
            .IsRequired();

        builder.Property(p => p.DailyRate)
            .IsRequired()
            .HasColumnType("decimal(10, 2)");

        builder.Property(p => p.PenaltyPercentage)
            .IsRequired()
            .HasColumnType("decimal(5, 2)");

        builder.Property(p => p.CreationDate)
            .IsRequired();

        builder.Property(p => p.UpdateDate)
            .IsRequired();

        builder.HasData(
            Plan.CreateSevenDayPlan(),
            Plan.CreateFifteenDayPlan(),
            Plan.CreateThirtyDayPlan(),
            Plan.CreateFortyFiveDayPlan(),
            Plan.CreateFiftyDayPlan()
        );
    }
}
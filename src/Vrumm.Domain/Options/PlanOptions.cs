namespace Vrumm.Domain.Options;
public class PlanOptions
{
    public const string SectionName = "Plans";

    public List<PlanConfig> Configurations { get; set; } = new List<PlanConfig>();

    public class PlanConfig
    {
        public int DayCount { get; set; }
        public decimal DailyRate { get; set; }
        public decimal PenaltyPercentage { get; set; }
        public decimal AdditionalDayRate { get; set; }
    }
}
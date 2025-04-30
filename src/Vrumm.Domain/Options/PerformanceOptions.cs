namespace Vrumm.Domain.Options;
public class PerformanceOptions
{
    public const string SectionName = "Performance";
    public int ThresholdMs { get; set; } = 500;
}
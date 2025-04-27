namespace Vrumm.Application.Plans.Dtos;
public class PlanDto
{
    public int Id { get; set; }
    public int DayCount { get; set; }
    public decimal DailyRate { get; set; }
    public decimal PenaltyPercentage { get; set; }
    public decimal TotalValue => DayCount * DailyRate;
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
}
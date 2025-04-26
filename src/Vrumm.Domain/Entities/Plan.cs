using Vrumm.Domain.Common;
using Vrumm.Domain.Common.Exceptions;

namespace Vrumm.Domain.Entities;
public class Plan : Entity<int>
{
    public int DayCount { get; private set; }
    public decimal DailyRate { get; private set; }
    public decimal PenaltyPercentage { get; private set; }

    private Plan() { }

    public Plan(int id, int dayCount, decimal dailyRate, decimal penaltyPercentage)
    {
        if (dayCount <= 0)
            throw new DomainException("Quantidade de dias deve ser maior que zero");

        if (dailyRate <= 0)
            throw new DomainException("Valor diário deve ser maior que zero");

        if (penaltyPercentage < 0 || penaltyPercentage > 100)
            throw new DomainException("Percentual de multa deve estar entre 0 e 100");

        Id = id;
        DayCount = dayCount;
        DailyRate = dailyRate;
        PenaltyPercentage = penaltyPercentage;
    }

    public decimal CalculateTotalValue()
    {
        return DayCount * DailyRate;
    }

    public decimal CalculateEarlyReturnPenalty(int actualDays)
    {
        if (actualDays >= DayCount)
            return 0;

        var remainingDays = DayCount - actualDays;
        var remainingValue = remainingDays * DailyRate;

        return remainingValue * (PenaltyPercentage / 100);
    }

    public decimal CalculateAdditionalDaysValue(int additionalDays)
    {
        if (additionalDays <= 0)
            return 0;

        return additionalDays * (DailyRate * 1.5m);
    }

    public static Plan CreateSevenDayPlan()
    {
        return new Plan(1, 7, 30.00m, 20.00m);
    }

    public static Plan CreateFifteenDayPlan()
    {
        return new Plan(2, 15, 28.00m, 25.00m);
    }

    public static Plan CreateThirtyDayPlan()
    {
        return new Plan(3, 30, 25.00m, 30.00m);
    }

    public static Plan CreateFortyFiveDayPlan()
    {
        return new Plan(4, 45, 23.00m, 35.00m);
    }

    public static Plan CreateFiftyDayPlan()
    {
        return new Plan(5, 50, 22.00m, 40.00m);
    }
}
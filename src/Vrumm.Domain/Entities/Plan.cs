using Vrumm.Domain.Common;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities;
public class Plan : Entity<int>
{
    public int DayCount { get; private set; }
    public decimal DailyRate { get; private set; }
    public decimal PenaltyPercentage { get; private set; }
    public decimal AdditionalDayRate { get; private set; }

    private Plan() { }

    public Plan(int id, int dayCount, decimal dailyRate, decimal penaltyPercentage, decimal additionalDayRate)
    {
        if (dayCount <= 0)
            throw new DomainException("Quantidade de dias deve ser maior que zero");
        if (dailyRate <= 0)
            throw new DomainException("Valor diário deve ser maior que zero");
        if (penaltyPercentage < 0 || penaltyPercentage > 100)
            throw new DomainException("Percentual de multa deve estar entre 0 e 100");
        if (additionalDayRate < 0)
            throw new DomainException("Valor para dias adicionais não pode ser negativo");

        Id = id;
        DayCount = dayCount;
        DailyRate = dailyRate;
        PenaltyPercentage = penaltyPercentage;
        AdditionalDayRate = additionalDayRate;
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
        return additionalDays * AdditionalDayRate;
    }
}
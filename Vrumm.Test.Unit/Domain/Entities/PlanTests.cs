using FluentAssertions;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities;
public class PlanTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesPlan()
    {
        // Arrange
        var id = 1;
        var dayCount = 7;
        var dailyRate = 30m;
        var penaltyPercentage = 20m;
        var additionalDayRate = 50m;

        // Act
        var plan = new Plan(id, dayCount, dailyRate, penaltyPercentage, additionalDayRate);

        // Assert
        plan.Id.Should().Be(id);
        plan.DayCount.Should().Be(dayCount);
        plan.DailyRate.Should().Be(dailyRate);
        plan.PenaltyPercentage.Should().Be(penaltyPercentage);
        plan.AdditionalDayRate.Should().Be(additionalDayRate);
    }

    [Fact]
    public void Constructor_ZeroDayCount_ThrowsDomainException()
    {
        // Act
        Action act = () => new Plan(1, 0, 30m, 20m, 50m);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Quantidade de dias deve ser maior que zero");
    }

    [Fact]
    public void Constructor_NegativeDailyRate_ThrowsDomainException()
    {
        // Act
        Action act = () => new Plan(1, 7, -30m, 20m, 50m);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Valor diário deve ser maior que zero");
    }

    [Fact]
    public void Constructor_InvalidPenaltyPercentage_ThrowsDomainException()
    {
        // Act
        Action act = () => new Plan(1, 7, 30m, 101m, 50m);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Percentual de multa deve estar entre 0 e 100");
    }

    [Fact]
    public void CalculateTotalValue_ReturnsCorrectValue()
    {
        // Arrange
        var plan = new Plan(1, 7, 30m, 20m, 50m);

        // Act
        var totalValue = plan.CalculateTotalValue();

        // Assert
        totalValue.Should().Be(7 * 30m); // 210
    }

    [Fact]
    public void CalculateEarlyReturnPenalty_ActualDaysLessThanPlan_ReturnsPenalty()
    {
        // Arrange
        var plan = new Plan(1, 7, 30m, 20m, 50m);
        var actualDays = 5;

        // Act
        var penalty = plan.CalculateEarlyReturnPenalty(actualDays);

        // Assert
        var remainingDays = 7 - 5;
        var remainingValue = remainingDays * 30m;
        var expectedPenalty = remainingValue * (20m / 100);
        penalty.Should().Be(expectedPenalty); // (2 * 30) * 0.2 = 12
    }

    [Fact]
    public void CalculateEarlyReturnPenalty_ActualDaysEqualOrGreaterThanPlan_ReturnsZero()
    {
        // Arrange
        var plan = new Plan(1, 7, 30m, 20m, 50m);

        // Act
        var penalty1 = plan.CalculateEarlyReturnPenalty(7);
        var penalty2 = plan.CalculateEarlyReturnPenalty(8);

        // Assert
        penalty1.Should().Be(0m);
        penalty2.Should().Be(0m);
    }

    [Fact]
    public void CalculateAdditionalDaysValue_ValidAdditionalDays_ReturnsCorrectValue()
    {
        // Arrange
        var plan = new Plan(1, 7, 30m, 20m, 50m);
        var additionalDays = 3;

        // Act
        var additionalValue = plan.CalculateAdditionalDaysValue(additionalDays);

        // Assert
        additionalValue.Should().Be(3 * 50m); // 150
    }

    [Fact]
    public void CalculateAdditionalDaysValue_ZeroOrNegativeDays_ReturnsZero()
    {
        // Arrange
        var plan = new Plan(1, 7, 30m, 20m, 50m);

        // Act
        var value1 = plan.CalculateAdditionalDaysValue(0);
        var value2 = plan.CalculateAdditionalDaysValue(-1);

        // Assert
        value1.Should().Be(0m);
        value2.Should().Be(0m);
    }
}
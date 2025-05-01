using FluentAssertions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities;
public class RentalTests
{
    private readonly Guid _validMotorcycleId = Guid.NewGuid();
    private readonly Guid _validDriverId = Guid.NewGuid();
    private readonly int _validPlanId = 1;
    private readonly DateTime _validStartDate = DateTime.Today.AddDays(1);
    private readonly DateTime _validExpectedEndDate = DateTime.Today.AddDays(8);
    private readonly Plan _validPlan = new Plan(1, 7, 30m, 20m, 50m);

    [Fact]
    public void Constructor_ValidParameters_CreatesRental()
    {
        // Act
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);

        // Assert
        rental.Id.Should().NotBe(Guid.Empty);
        rental.MotorcycleId.Should().Be(_validMotorcycleId);
        rental.DriverId.Should().Be(_validDriverId);
        rental.PlanId.Should().Be(_validPlanId);
        rental.StartDate.Should().Be(_validStartDate);
        rental.ExpectedEndDate.Should().Be(_validExpectedEndDate);
        rental.Status.Should().Be(RentalStatus.Active);
        rental.EndDate.Should().BeNull();
        rental.TotalValue.Should().BeNull();
    }

    [Fact]
    public void Constructor_InvalidMotorcycleId_ThrowsDomainException()
    {
        // Act
        Action act = () => new Rental(Guid.Empty, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("ID da moto inválido");
    }

    [Fact]
    public void Constructor_PastStartDate_ThrowsDomainException()
    {
        // Arrange
        var pastStartDate = DateTime.Today.AddDays(-1);

        // Act
        Action act = () => new Rental(_validMotorcycleId, _validDriverId, _validPlanId, pastStartDate, _validExpectedEndDate);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Data de início não pode ser no passado");
    }

    [Fact]
    public void CalculateReturnValue_OnExpectedEndDate_ReturnsTotalValue()
    {
        // Arrange
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);

        // Act
        var value = rental.CalculateReturnValue(_validExpectedEndDate, _validPlan);

        // Assert
        value.Should().Be(_validPlan.CalculateTotalValue()); // 7 * 30 = 210
    }

    [Fact]
    public void CalculateReturnValue_EarlyReturn_IncludesPenalty()
    {
        // Arrange
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);
        var earlyReturnDate = _validStartDate.AddDays(5); // 6 dias reais

        // Act
        var value = rental.CalculateReturnValue(earlyReturnDate, _validPlan);

        // Assert
        var actualDays = 6;
        var baseValue = actualDays * _validPlan.DailyRate; // 6 * 30 = 180
        var penalty = _validPlan.CalculateEarlyReturnPenalty(actualDays); // (7-6) * 30 * 0.2 = 6
        value.Should().Be(baseValue + penalty); // 180 + 6 = 186
    }

    [Fact]
    public void CalculateReturnValue_LateReturn_IncludesAdditionalDays()
    {
        // Arrange
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);
        var lateReturnDate = _validExpectedEndDate.AddDays(2); // 2 dias extras

        // Act
        var value = rental.CalculateReturnValue(lateReturnDate, _validPlan);

        // Assert
        var normalValue = _validPlan.CalculateTotalValue(); // 7 * 30 = 210
        var additionalValue = _validPlan.CalculateAdditionalDaysValue(2); // 2 * 50 = 100
        value.Should().Be(normalValue + additionalValue); // 210 + 100 = 310
    }

    [Fact]
    public void FinalizeRental_ValidParameters_FinalizesRental()
    {
        // Arrange
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);
        var returnDate = _validExpectedEndDate;
        var totalValue = 210m;

        // Act
        rental.FinalizeRental(returnDate, totalValue);

        // Assert
        rental.Status.Should().Be(RentalStatus.Finalized);
        rental.EndDate.Should().Be(returnDate);
        rental.TotalValue.Should().Be(totalValue);
        rental.UpdateDate.Should().BeAfter(rental.CreationDate);
    }

    [Fact]
    public void FinalizeRental_NonActiveRental_ThrowsDomainException()
    {
        // Arrange
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);
        rental.CancelRental();

        // Act
        Action act = () => rental.FinalizeRental(_validExpectedEndDate, 210m);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Não é possível finalizar uma locação não ativa");
    }

    [Fact]
    public void GenerateCreatedEvent_NotEmitted_ReturnsEvent()
    {
        // Arrange
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);

        // Act
        var @event = rental.GenerateCreatedEvent();

        // Assert
        @event.RentalId.Should().Be(rental.Id);
        @event.MotorcycleId.Should().Be(_validMotorcycleId);
        @event.DriverId.Should().Be(_validDriverId);
        @event.PlanId.Should().Be(_validPlanId);
        @event.StartDate.Should().Be(_validStartDate);
        @event.ExpectedEndDate.Should().Be(_validExpectedEndDate);
    }

    [Fact]
    public void GenerateCreatedEvent_AlreadyEmitted_ThrowsDomainException()
    {
        // Arrange
        var rental = new Rental(_validMotorcycleId, _validDriverId, _validPlanId, _validStartDate, _validExpectedEndDate);
        rental.GenerateCreatedEvent();

        // Act
        Action act = () => rental.GenerateCreatedEvent();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Evento de criação já foi emitido para esta locação.");
    }
}
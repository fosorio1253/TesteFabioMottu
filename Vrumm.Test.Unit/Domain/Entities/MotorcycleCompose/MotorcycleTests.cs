using FluentAssertions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.MotorcycleCompose;
public class MotorcycleTests
{
    private readonly MotorcycleModel _validModel = MotorcycleModel.Create("Honda CB500");
    private readonly ManufactureYear _validYear = ManufactureYear.Create(2020);
    private readonly LicensePlate _validLicensePlate = LicensePlate.Create("ABC1D23");

    [Fact]
    public void Constructor_ValidParameters_CreatesMotorcycle()
    {
        // Act
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);

        // Assert
        motorcycle.Id.Should().NotBe(Guid.Empty);
        motorcycle.Status().Should().Be(MotorcycleStatus.Available);
        motorcycle.Model().Should().Be("Honda CB500");
        motorcycle.Year().Should().Be(2020);
        motorcycle.LicensePlate().Should().Be("ABC1D23");
    }

    [Fact]
    public void Rent_AvailableMotorcycle_TransitionsToRented()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);

        // Act
        motorcycle.Rent();

        // Assert
        motorcycle.Status().Should().Be(MotorcycleStatus.Rented);
        motorcycle.UpdateDate.Should().BeAfter(motorcycle.CreationDate);
    }

    [Fact]
    public void Rent_NonAvailableMotorcycle_ThrowsMotorcycleNotAvailableException()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);
        motorcycle.Rent(); // Já alugada

        // Act
        Action act = () => motorcycle.Rent();

        // Assert
        act.Should().Throw<MotorcycleNotAvailableException>()
           .WithMessage("Moto não está disponível para aluguel.");
    }

    [Fact]
    public void Return_RentedMotorcycle_TransitionsToAvailable()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);
        motorcycle.Rent();

        // Act
        motorcycle.Return();

        // Assert
        motorcycle.Status().Should().Be(MotorcycleStatus.Available);
    }

    [Fact]
    public void Return_NonRentedMotorcycle_ThrowsDomainException()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);

        // Act
        Action act = () => motorcycle.Return();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Esta moto não está alugada.");
    }

    [Fact]
    public void CanBeRemoved_NonRentedOrUnderMaintenance_ReturnsTrue()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);

        // Act
        var canBeRemoved = motorcycle.CanBeRemoved();

        // Assert
        canBeRemoved.Should().BeTrue();
    }

    [Fact]
    public void CanBeRemoved_RentedMotorcycle_ReturnsFalse()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);
        motorcycle.Rent();

        // Act
        var canBeRemoved = motorcycle.CanBeRemoved();

        // Assert
        canBeRemoved.Should().BeFalse();
    }

    [Fact]
    public void CanBeDeleted_NoRentals_ReturnsTrue()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);
        var rentals = new List<Rental>();

        // Act
        var canBeDeleted = motorcycle.CanBeDeleted(rentals);

        // Assert
        canBeDeleted.Should().BeTrue();
    }

    [Fact]
    public void CanBeDeleted_WithRentals_ReturnsFalse()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);
        var rentals = new List<Rental> { new Rental(Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.Today, DateTime.Today.AddDays(7)) };

        // Act
        var canBeDeleted = motorcycle.CanBeDeleted(rentals);

        // Assert
        canBeDeleted.Should().BeFalse();
    }

    [Fact]
    public void GenerateRegisteredEvent_ReturnsCorrectEvent()
    {
        // Arrange
        var motorcycle = new Motorcycle(_validModel, _validYear, _validLicensePlate);

        // Act
        var @event = motorcycle.GenerateRegisteredEvent();

        // Assert
        @event.MotorcycleId.Should().Be(motorcycle.Id);
        @event.Model.Should().Be("Honda CB500");
        @event.Year.Should().Be(2020);
        @event.LicensePlate.Should().Be("ABC1D23");
    }
}
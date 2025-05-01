using FluentAssertions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.MotorcycleCompose;
public class MotorcycleStatusStateTests
{
    [Fact]
    public void Available_CreatesInstanceWithAvailableStatus()
    {
        // Act
        var status = MotorcycleStatusState.Available();

        // Assert
        status.ToStatus().Should().Be(MotorcycleStatus.Available);
    }

    [Fact]
    public void TransitionToRent_FromAvailable_ReturnsRented()
    {
        // Arrange
        var status = MotorcycleStatusState.Available();

        // Act
        var newStatus = status.TransitionToRent();

        // Assert
        newStatus.ToStatus().Should().Be(MotorcycleStatus.Rented);
    }

    [Fact]
    public void TransitionToRent_FromNonAvailable_ThrowsMotorcycleNotAvailableException()
    {
        // Arrange
        var status = MotorcycleStatusState.Rented();

        // Act
        Action act = () => status.TransitionToRent();

        // Assert
        act.Should().Throw<MotorcycleNotAvailableException>()
           .WithMessage("Moto não está disponível para aluguel.");
    }

    [Fact]
    public void TransitionToReturn_FromRented_ReturnsAvailable()
    {
        // Arrange
        var status = MotorcycleStatusState.Rented();

        // Act
        var newStatus = status.TransitionToReturn();

        // Assert
        newStatus.ToStatus().Should().Be(MotorcycleStatus.Available);
    }

    [Fact]
    public void TransitionToReturn_FromNonRented_ThrowsDomainException()
    {
        // Arrange
        var status = MotorcycleStatusState.Available();

        // Act
        Action act = () => status.TransitionToReturn();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Esta moto não está alugada.");
    }

    [Fact]
    public void TransitionToUnderMaintenance_FromAvailable_ReturnsUnderMaintenance()
    {
        // Arrange
        var status = MotorcycleStatusState.Available();

        // Act
        var newStatus = status.TransitionToUnderMaintenance();

        // Assert
        newStatus.ToStatus().Should().Be(MotorcycleStatus.UnderMaintenance);
    }

    [Fact]
    public void TransitionToUnderMaintenance_FromRented_ThrowsDomainException()
    {
        // Arrange
        var status = MotorcycleStatusState.Rented();

        // Act
        Action act = () => status.TransitionToUnderMaintenance();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Não é possível colocar uma moto alugada em manutenção.");
    }

    [Fact]
    public void TransitionToInactive_FromAvailable_ReturnsInactive()
    {
        // Arrange
        var status = MotorcycleStatusState.Available();

        // Act
        var newStatus = status.TransitionToInactive();

        // Assert
        newStatus.ToStatus().Should().Be(MotorcycleStatus.Inactive);
    }

    [Fact]
    public void TransitionToInactive_FromRented_ThrowsDomainException()
    {
        // Arrange
        var status = MotorcycleStatusState.Rented();

        // Act
        Action act = () => status.TransitionToInactive();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Não é possível inativar uma moto alugada.");
    }

    [Fact]
    public void CanBeRemoved_NonRentedOrUnderMaintenance_ReturnsTrue()
    {
        // Arrange
        var status = MotorcycleStatusState.Available();

        // Act
        var canBeRemoved = status.CanBeRemoved();

        // Assert
        canBeRemoved.Should().BeTrue();
    }

    [Fact]
    public void CanBeRemoved_RentedOrUnderMaintenance_ReturnsFalse()
    {
        // Arrange
        var rentedStatus = MotorcycleStatusState.Rented();
        var maintenanceStatus = MotorcycleStatusState.UnderMaintenance();

        // Act
        var canBeRemovedRented = rentedStatus.CanBeRemoved();
        var canBeRemovedMaintenance = maintenanceStatus.CanBeRemoved();

        // Assert
        canBeRemovedRented.Should().BeFalse();
        canBeRemovedMaintenance.Should().BeFalse();
    }

    [Fact]
    public void CanBeDeleted_AvailableWithNoRentals_ReturnsTrue()
    {
        // Arrange
        var status = MotorcycleStatusState.Available();
        var rentals = new List<Rental>();

        // Act
        var canBeDeleted = status.CanBeDeleted(rentals);

        // Assert
        canBeDeleted.Should().BeTrue();
    }

    [Fact]
    public void CanBeDeleted_NonAvailableOrWithRentals_ReturnsFalse()
    {
        // Arrange
        var rentedStatus = MotorcycleStatusState.Rented();
        var availableStatus = MotorcycleStatusState.Available();
        var rentals = new List<Rental> { new Rental(Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.Today, DateTime.Today.AddDays(7)) };

        // Act
        var canBeDeletedRented = rentedStatus.CanBeDeleted(rentals);
        var canBeDeletedAvailableWithRentals = availableStatus.CanBeDeleted(rentals);

        // Assert
        canBeDeletedRented.Should().BeFalse();
        canBeDeletedAvailableWithRentals.Should().BeFalse();
    }
}
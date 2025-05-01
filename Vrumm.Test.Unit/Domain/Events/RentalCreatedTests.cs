using FluentAssertions;
using Vrumm.Domain.Events;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Events;
public class RentalCreatedTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var motorcycleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var planId = 1;
        var startDate = DateTime.Today.AddDays(1);
        var expectedEndDate = DateTime.Today.AddDays(8);

        // Act
        var @event = new RentalCreated(rentalId, motorcycleId, driverId, planId, startDate, expectedEndDate);

        // Assert
        @event.RentalId.Should().Be(rentalId);
        @event.MotorcycleId.Should().Be(motorcycleId);
        @event.DriverId.Should().Be(driverId);
        @event.PlanId.Should().Be(planId);
        @event.StartDate.Should().Be(startDate);
        @event.ExpectedEndDate.Should().Be(expectedEndDate);
        @event.Id.Should().NotBe(Guid.Empty);
        @event.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
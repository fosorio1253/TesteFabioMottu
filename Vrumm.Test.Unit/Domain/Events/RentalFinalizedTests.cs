using FluentAssertions;
using Vrumm.Domain.Events;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Events;
public class RentalFinalizedTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var motorcycleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var endDate = DateTime.Today;
        var totalValue = 210m;

        // Act
        var @event = new RentalFinalized(rentalId, motorcycleId, driverId, endDate, totalValue);

        // Assert
        @event.RentalId.Should().Be(rentalId);
        @event.MotorcycleId.Should().Be(motorcycleId);
        @event.DriverId.Should().Be(driverId);
        @event.EndDate.Should().Be(endDate);
        @event.TotalValue.Should().Be(totalValue);
        @event.Id.Should().NotBe(Guid.Empty);
        @event.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
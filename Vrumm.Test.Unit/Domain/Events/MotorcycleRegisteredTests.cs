using FluentAssertions;
using Vrumm.Domain.Events;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Events;
public class MotorcycleRegisteredTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var model = "Honda CB500";
        var year = 2020;
        var licensePlate = "ABC1D23";

        // Act
        var @event = new MotorcycleRegistered(motorcycleId, model, year, licensePlate);

        // Assert
        @event.MotorcycleId.Should().Be(motorcycleId);
        @event.Model.Should().Be(model);
        @event.Year.Should().Be(year);
        @event.LicensePlate.Should().Be(licensePlate);
        @event.Id.Should().NotBe(Guid.Empty);
        @event.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
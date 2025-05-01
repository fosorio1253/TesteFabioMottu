using FluentAssertions;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.MotorcycleCompose;
public class MotorcycleRegistrationEventTests
{
    [Fact]
    public void Constructor_ValidParameters_SetsProperties()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var year = 2020;
        var model = "Honda CB500";
        var licensePlate = "ABC1D23";
        var eventTimestamp = DateTime.UtcNow;

        // Act
        var registrationEvent = new MotorcycleRegistrationEvent(motorcycleId, year, model, licensePlate, eventTimestamp);

        // Assert
        registrationEvent.Id.Should().NotBe(Guid.Empty);
        registrationEvent.MotorcycleId.Should().Be(motorcycleId);
        registrationEvent.Year.Should().Be(year);
        registrationEvent.Model.Should().Be(model);
        registrationEvent.LicensePlate.Should().Be(licensePlate);
        registrationEvent.EventTimestamp.Should().Be(eventTimestamp);
        registrationEvent.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        registrationEvent.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        // Arrange
        var event1 = new MotorcycleRegistrationEvent
            (Guid.NewGuid(), 2020, "Honda CB500", "ABC1D23", DateTime.UtcNow);
        var event2 = new MotorcycleRegistrationEvent
            (event1.Id, 2021, "Yamaha MT-03", "XYZ9W87", DateTime.UtcNow);

        // Act
        var areEqual = event1.Equals(event2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        // Arrange
        var event1 = new MotorcycleRegistrationEvent
            (Guid.NewGuid(), 2020, "Honda CB500", "ABC1D23", DateTime.UtcNow);
        var event2 = new MotorcycleRegistrationEvent
            (Guid.NewGuid(), 2020, "Honda CB500", "ABC1D23", DateTime.UtcNow);

        // Act
        var areEqual = event1.Equals(event2);

        // Assert
        areEqual.Should().BeFalse();
    }
}

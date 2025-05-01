using FluentAssertions;
using Vrumm.Domain.Exceptions.Motorcycles;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Motorcycles;
public class MotorcycleHasRentalsExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsDefaultMessage()
    {
        // Act
        var exception = new MotorcycleHasRentalsException();

        // Assert
        exception.Message.Should().Be("Cannot delete motorcycle with rental history");
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Custom rental history error";

        // Act
        var exception = new MotorcycleHasRentalsException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var message = "Custom rental history error";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new MotorcycleHasRentalsException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Constructor_WithMotorcycleId_SetsMessage()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();

        // Act
        var exception = new MotorcycleHasRentalsException(motorcycleId);

        // Assert
        exception.Message.Should().Be($"Cannot delete motorcycle with ID {motorcycleId} because it has rental history");
    }
}
using FluentAssertions;
using Vrumm.Domain.Exceptions.Motorcycles;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Motorcycles;
public class MotorcycleNotAvailableExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Motorcycle not available";

        // Act
        var exception = new MotorcycleNotAvailableException(message);

        // Assert
        exception.Message.Should().Be(message);
    }
}
using FluentAssertions;
using Vrumm.Domain.Exceptions.Motorcycles;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Motorcycles;
public class InvalidLicensePlateExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Invalid license plate";

        // Act
        var exception = new InvalidLicensePlateException(message);

        // Assert
        exception.Message.Should().Be(message);
    }
}
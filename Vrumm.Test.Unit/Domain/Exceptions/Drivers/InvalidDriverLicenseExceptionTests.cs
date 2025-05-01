using FluentAssertions;
using Vrumm.Domain.Exceptions.Drivers;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Drivers;
public class InvalidDriverLicenseExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Invalid driver license";

        // Act
        var exception = new InvalidDriverLicenseException(message);

        // Assert
        exception.Message.Should().Be(message);
    }
}
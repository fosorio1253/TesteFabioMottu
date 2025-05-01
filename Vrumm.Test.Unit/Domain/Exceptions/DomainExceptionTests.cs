using FluentAssertions;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions;
public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Test error";

        // Act
        var exception = new DomainException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var message = "Test error";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new DomainException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}

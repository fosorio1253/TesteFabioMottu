using FluentAssertions;
using Vrumm.Application.Common.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions;
public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithNameAndKey_SetsMessage()
    {
        // Arrange
        var name = "Entity";
        var key = "123";

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be($"Entity \"{name}\" with key \"{key}\" was not found.");
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Custom not found error";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var message = "Custom not found error";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new NotFoundException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}
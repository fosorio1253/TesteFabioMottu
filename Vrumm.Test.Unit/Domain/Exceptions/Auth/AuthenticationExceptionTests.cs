using FluentAssertions;
using Vrumm.Domain.Exceptions.Auth;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Auth;
public class AuthenticationExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsDefaultMessage()
    {
        // Act
        var exception = new AuthenticationException();

        // Assert
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Authentication failed";

        // Act
        var exception = new AuthenticationException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var message = "Authentication failed";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new AuthenticationException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}
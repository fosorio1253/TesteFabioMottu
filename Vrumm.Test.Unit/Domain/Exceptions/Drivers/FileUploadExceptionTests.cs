using FluentAssertions;
using Vrumm.Domain.Exceptions.Drivers;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Drivers;
public class FileUploadExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "File upload failed";

        // Act
        var exception = new FileUploadException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.FileName.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var message = "File upload failed";
        var innerException = new IOException("IO error");

        // Act
        var exception = new FileUploadException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.FileName.Should().Be(innerException.Source);
    }

    [Fact]
    public void Constructor_WithFileNameAndMessage_SetsProperties()
    {
        // Arrange
        var fileName = "test.png";
        var message = "File upload failed";

        // Act
        var exception = new FileUploadException(fileName, message);

        // Assert
        exception.Message.Should().Be(message);
        exception.FileName.Should().Be(fileName);
    }

    [Fact]
    public void Constructor_WithFileNameMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var fileName = "test.png";
        var message = "File upload failed";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new FileUploadException(fileName, message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.FileName.Should().Be(fileName);
        exception.InnerException.Should().Be(innerException);
    }
}
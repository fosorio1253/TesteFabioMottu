using FluentAssertions;
using Vrumm.Domain.Exceptions.Drivers;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Drivers;
public class InvalidFileTypeExceptionTests
{
    [Fact]
    public void Constructor_WithFileType_SetsMessage()
    {
        // Arrange
        var fileType = "jpg";

        // Act
        var exception = new InvalidFileTypeException(fileType);

        // Assert
        exception.Message.Should().Be($"The file type \"{fileType}\" is not allowed.");
    }
}
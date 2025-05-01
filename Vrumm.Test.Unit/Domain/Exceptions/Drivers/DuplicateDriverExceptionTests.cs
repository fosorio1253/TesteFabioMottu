using FluentAssertions;
using Vrumm.Domain.Exceptions.Drivers;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Drivers;
public class DuplicateDriverExceptionTests
{
    [Fact]
    public void Constructor_WithCnpj_SetsMessage()
    {
        // Arrange
        var cnpj = "12345678000195";

        // Act
        var exception = new DuplicateDriverException(cnpj);

        // Assert
        exception.Message.Should().Be($"A driver with CNPJ {cnpj} already exists.");
    }

    [Fact]
    public void Constructor_WithCnpjAndInnerException_SetsProperties()
    {
        // Arrange
        var cnpj = "12345678000195";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new DuplicateDriverException(cnpj, innerException);

        // Assert
        exception.Message.Should().Be($"A driver with CNPJ {cnpj} already exists.");
        exception.InnerException.Should().Be(innerException);
    }
}
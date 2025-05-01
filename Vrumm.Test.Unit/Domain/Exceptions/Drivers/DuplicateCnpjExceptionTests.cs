using FluentAssertions;
using Vrumm.Domain.Exceptions.Drivers;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Drivers;
public class DuplicateCnpjExceptionTests
{
    [Fact]
    public void Constructor_WithTaxId_SetsMessage()
    {
        // Arrange
        var taxId = "12345678000195";

        // Act
        var exception = new DuplicateCnpjException(taxId);

        // Assert
        exception.Message.Should().Be($"A driver with tax ID \"{taxId}\" already exists.");
    }
}

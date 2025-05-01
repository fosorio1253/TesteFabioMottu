using FluentAssertions;
using Vrumm.Domain.Exceptions.Drivers;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Drivers;
public class DuplicateLicenseNumberExceptionTests
{
    [Fact]
    public void Constructor_WithLicenseNumber_SetsMessage()
    {
        // Arrange
        var licenseNumber = "12345678901";

        // Act
        var exception = new DuplicateLicenseNumberException(licenseNumber);

        // Assert
        exception.Message.Should().Be($"A driver with license number {licenseNumber} already exists.");
    }

    [Fact]
    public void Constructor_WithLicenseNumberAndInnerException_SetsProperties()
    {
        // Arrange
        var licenseNumber = "12345678901";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new DuplicateLicenseNumberException(licenseNumber, innerException);

        // Assert
        exception.Message.Should().Be($"A driver with license number {licenseNumber} already exists.");
        exception.InnerException.Should().Be(innerException);
    }
}
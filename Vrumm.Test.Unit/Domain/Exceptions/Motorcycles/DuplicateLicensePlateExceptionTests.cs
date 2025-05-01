using FluentAssertions;
using Vrumm.Domain.Exceptions.Motorcycles;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Exceptions.Motorcycles;
public class DuplicateLicensePlateExceptionTests
{
    [Fact]
    public void Constructor_WithLicensePlate_SetsProperties()
    {
        // Arrange
        var licensePlate = "ABC1D23";

        // Act
        var exception = new DuplicateLicensePlateException(licensePlate);

        // Assert
        exception.Message.Should().Be($"Já existe uma moto com a placa {licensePlate}");
        exception.LicensePlate.Should().Be(licensePlate);
    }

    [Fact]
    public void Constructor_WithLicensePlateAndInnerException_SetsProperties()
    {
        // Arrange
        var licensePlate = "ABC1D23";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new DuplicateLicensePlateException(licensePlate, innerException);

        // Assert
        exception.Message.Should().Be($"Já existe uma moto com a placa {licensePlate}");
        exception.LicensePlate.Should().Be(licensePlate);
        exception.InnerException.Should().Be(innerException);
    }
}
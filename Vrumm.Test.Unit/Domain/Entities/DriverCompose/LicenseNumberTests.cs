using FluentAssertions;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.DriverCompose;
public class LicenseNumberTests
{
    [Fact]
    public void Create_ValidLicenseNumber_ReturnsInstance()
    {
        // Arrange
        var validNumber = "12345678901";

        // Act
        var licenseNumber = LicenseNumber.Create(validNumber);

        // Assert
        licenseNumber.GetValue().Should().Be(validNumber);
        licenseNumber.ToStringRepresentation().Should().Be(validNumber);
        licenseNumber.ToString().Should().Be($"LicenseNumber: {validNumber}");
    }

    [Fact]
    public void Create_EmptyLicenseNumber_ThrowsDomainException()
    {
        // Arrange
        var emptyNumber = "";

        // Act
        Action act = () => LicenseNumber.Create(emptyNumber);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("License number cannot be empty.");
    }

    [Fact]
    public void Create_LicenseNumberExceeding20Characters_ThrowsDomainException()
    {
        // Arrange
        var longNumber = "123456789012345678901";

        // Act
        Action act = () => LicenseNumber.Create(longNumber);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("License number must not exceed 20 characters.");
    }

    [Fact]
    public void Equals_SameLicenseNumber_ReturnsTrue()
    {
        // Arrange
        var number1 = LicenseNumber.Create("12345678901");
        var number2 = LicenseNumber.Create("12345678901");

        // Act
        var areEqual = number1.Equals(number2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentLicenseNumber_ReturnsFalse()
    {
        // Arrange
        var number1 = LicenseNumber.Create("12345678901");
        var number2 = LicenseNumber.Create("98765432109");

        // Act
        var areEqual = number1.Equals(number2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameLicenseNumber_ReturnsSameHash()
    {
        // Arrange
        var number1 = LicenseNumber.Create("12345678901");
        var number2 = LicenseNumber.Create("12345678901");

        // Act
        var hash1 = number1.GetHashCode();
        var hash2 = number2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}

using FluentAssertions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.DriverCompose;
public class LicenseTypeValueTests
{
    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("AB")]
    public void Create_ValidLicenseTypeString_ReturnsInstance(string type)
    {
        // Act
        var licenseType = LicenseTypeValue.Create(type);

        // Assert
        licenseType.Value.ToString().Should().Be(type);
        licenseType.ToString().Should().Be(type);
    }

    [Theory]
    [InlineData(LicenseType.A)]
    [InlineData(LicenseType.B)]
    [InlineData(LicenseType.AB)]
    public void Create_ValidLicenseTypeEnum_ReturnsInstance(LicenseType type)
    {
        // Act
        var licenseType = LicenseTypeValue.Create(type);

        // Assert
        licenseType.Value.Should().Be(type);
        licenseType.ToString().Should().Be(type.ToString());
    }

    [Fact]
    public void Create_EmptyLicenseType_ThrowsDomainException()
    {
        // Act
        Action act = () => LicenseTypeValue.Create("");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("License type cannot be empty.");
    }

    [Fact]
    public void Create_InvalidLicenseTypeString_ThrowsDomainException()
    {
        // Arrange
        var invalidType = "C";

        // Act
        Action act = () => LicenseTypeValue.Create(invalidType);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("License type must be A, B, or AB.");
    }

    [Fact]
    public void Equals_SameLicenseType_ReturnsTrue()
    {
        // Arrange
        var type1 = LicenseTypeValue.Create("A");
        var type2 = LicenseTypeValue.Create(LicenseType.A);

        // Act
        var areEqual = type1.Equals(type2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentLicenseType_ReturnsFalse()
    {
        // Arrange
        var type1 = LicenseTypeValue.Create("A");
        var type2 = LicenseTypeValue.Create("B");

        // Act
        var areEqual = type1.Equals(type2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameLicenseType_ReturnsSameHash()
    {
        // Arrange
        var type1 = LicenseTypeValue.Create("A");
        var type2 = LicenseTypeValue.Create("A");

        // Act
        var hash1 = type1.GetHashCode();
        var hash2 = type2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}
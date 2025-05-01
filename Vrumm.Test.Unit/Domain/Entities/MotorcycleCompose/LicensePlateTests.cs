using FluentAssertions;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Exceptions.Motorcycles;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.MotorcycleCompose;
public class LicensePlateTests
{
    [Fact]
    public void Create_ValidLicensePlate_ReturnsInstance()
    {
        // Arrange
        var validPlate = "ABC1D23";

        // Act
        var licensePlate = LicensePlate.Create(validPlate);

        // Assert
        licensePlate.GetValue().Should().Be(validPlate);
        licensePlate.ToStringRepresentation().Should().Be(validPlate);
        licensePlate.ToString().Should().Be($"LicensePlate: {validPlate}");
    }

    [Fact]
    public void Create_EmptyLicensePlate_ThrowsInvalidLicensePlateException()
    {
        // Act
        Action act = () => LicensePlate.Create("");

        // Assert
        act.Should().Throw<InvalidLicensePlateException>()
           .WithMessage("Placa não pode estar vazia");
    }

    [Fact]
    public void Create_InvalidFormatLicensePlate_ThrowsInvalidLicensePlateException()
    {
        // Arrange
        var invalidPlate = "ABCD123";

        // Act
        Action act = () => LicensePlate.Create(invalidPlate);

        // Assert
        act.Should().Throw<InvalidLicensePlateException>()
           .WithMessage("Formato de placa inválido");
    }

    [Fact]
    public void Create_LowerCaseLicensePlate_NormalizesToUpperCase()
    {
        // Arrange
        var lowerCasePlate = "abc1d23";

        // Act
        var licensePlate = LicensePlate.Create(lowerCasePlate);

        // Assert
        licensePlate.GetValue().Should().Be("ABC1D23");
    }

    [Fact]
    public void Equals_SameLicensePlate_ReturnsTrue()
    {
        // Arrange
        var plate1 = LicensePlate.Create("ABC1D23");
        var plate2 = LicensePlate.Create("ABC1D23");

        // Act
        var areEqual = plate1.Equals(plate2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentLicensePlate_ReturnsFalse()
    {
        // Arrange
        var plate1 = LicensePlate.Create("ABC1D23");
        var plate2 = LicensePlate.Create("XYZ9W87");

        // Act
        var areEqual = plate1.Equals(plate2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameLicensePlate_ReturnsSameHash()
    {
        // Arrange
        var plate1 = LicensePlate.Create("ABC1D23");
        var plate2 = LicensePlate.Create("ABC1D23");

        // Act
        var hash1 = plate1.GetHashCode();
        var hash2 = plate2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}

using FluentAssertions;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions.Drivers;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.DriverCompose;
public class DriverTests
{
    private readonly Cnpj _validCnpj = Cnpj.Create("12345678000195");
    private readonly BirthDate _validBirthDate = BirthDate.Create(new DateTime(1995, 5, 15));
    private readonly LicenseNumber _validLicenseNumber = LicenseNumber.Create("12345678901");
    private readonly LicenseTypeValue _validLicenseType = LicenseTypeValue.Create("A");
    private readonly string _validBase64Image = Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47 }); // PNG válido

    [Fact]
    public void Constructor_ValidParameters_CreatesDriver()
    {
        // Arrange
        var name = "John Doe";

        // Act
        var driver = new Driver(name, _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, _validBase64Image);

        // Assert
        driver.Name.Should().Be(name);
        driver.Cnpj.Should().Be(_validCnpj);
        driver.BirthDate.Should().Be(_validBirthDate);
        driver.LicenseNumber.Should().Be(_validLicenseNumber);
        driver.LicenseType.Should().Be(_validLicenseType);
        driver.ContentType.Should().Be("image/png");
        driver.FileName.Should().Be($"licenses/{driver.Id}_cnh.png");
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsDomainException()
    {
        // Act
        Action act = () => new Driver("", _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, _validBase64Image);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Driver name cannot be empty.");
    }

    [Fact]
    public void Constructor_EmptyImage_ThrowsDomainException()
    {
        // Act
        Action act = () => new Driver("John Doe", _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, "");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Imagem da CNH é obrigatória");
    }

    [Fact]
    public void Constructor_InvalidBase64Image_ThrowsDomainException()
    {
        // Arrange
        var invalidBase64 = "invalid-base64";

        // Act
        Action act = () => new Driver("John Doe", _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, invalidBase64);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Imagem da CNH deve ser um base64 válido");
    }

    [Fact]
    public void Constructor_NonPngOrBmpImage_ThrowsInvalidFileTypeException()
    {
        // Arrange
        var jpegBase64 = Convert.ToBase64String(new byte[] { 0xFF, 0xD8, 0xFF }); // JPEG inválido

        // Act
        Action act = () => new Driver("John Doe", _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, jpegBase64);

        // Assert
        act.Should().Throw<InvalidFileTypeException>()
           .WithMessage("Imagem da CNH deve ser PNG ou BMP");
    }

    [Fact]
    public void CanRentMotorcycle_LicenseTypeA_ReturnsTrue()
    {
        // Arrange
        var driver = new Driver("John Doe", _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, _validBase64Image);

        // Act
        var canRent = driver.CanRentMotorcycle();

        // Assert
        canRent.Should().BeTrue();
    }

    [Fact]
    public void CanRentMotorcycle_LicenseTypeB_ReturnsFalse()
    {
        // Arrange
        var licenseTypeB = LicenseTypeValue.Create("B");
        var driver = new Driver("John Doe", _validCnpj, _validBirthDate, _validLicenseNumber, licenseTypeB, _validBase64Image);

        // Act
        var canRent = driver.CanRentMotorcycle();

        // Assert
        canRent.Should().BeFalse();
    }

    [Fact]
    public void Update_ValidParameters_UpdatesFields()
    {
        // Arrange
        var driver = new Driver("John Doe", _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, _validBase64Image);
        var newName = "Jane Doe";
        var newCnpj = Cnpj.Create("98765432000188");
        var newBirthDate = BirthDate.Create(new DateTime(1990, 1, 1));
        var newLicenseNumber = LicenseNumber.Create("98765432109");
        var newLicenseType = LicenseTypeValue.Create("AB");

        // Act
        driver.Update(newName, newCnpj, newBirthDate, newLicenseNumber, newLicenseType);

        // Assert
        driver.Name.Should().Be(newName);
        driver.Cnpj.Should().Be(newCnpj);
        driver.BirthDate.Should().Be(newBirthDate);
        driver.LicenseNumber.Should().Be(newLicenseNumber);
        driver.LicenseType.Should().Be(newLicenseType);
        driver.UpdateDate.Should().BeAfter(driver.CreationDate);
    }

    [Fact]
    public void UpdateLicenseImage_ValidPath_UpdatesImagePath()
    {
        // Arrange
        var driver = new Driver("John Doe", _validCnpj, _validBirthDate, _validLicenseNumber, _validLicenseType, _validBase64Image);
        var newImagePath = "new/path/to/image.png";

        // Act
        driver.UpdateLicenseImage(newImagePath);

        // Assert
        driver.LicenseImagePath.Should().Be(newImagePath);
        driver.UpdateDate.Should().BeAfter(driver.CreationDate);
    }
}
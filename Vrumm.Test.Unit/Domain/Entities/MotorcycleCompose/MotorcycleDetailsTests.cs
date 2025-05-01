using FluentAssertions;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.MotorcycleCompose;
public class MotorcycleDetailsTests
{
    private readonly MotorcycleModel _validModel = MotorcycleModel.Create("Honda CB500");
    private readonly ManufactureYear _validYear = ManufactureYear.Create(2020);
    private readonly LicensePlate _validLicensePlate = LicensePlate.Create("ABC1D23");

    [Fact]
    public void Constructor_ValidParameters_SetsProperties()
    {
        // Act
        var details = new MotorcycleDetails(_validYear, _validModel, _validLicensePlate);

        // Assert
        details.Year().Should().Be(_validYear);
        details.Model().Should().Be(_validModel);
        details.LicensePlate().Should().Be(_validLicensePlate);
    }

    [Fact]
    public void Update_ValidParameters_ReturnsNewInstanceWithUpdatedValues()
    {
        // Arrange
        var details = new MotorcycleDetails(_validYear, _validModel, _validLicensePlate);
        var newModel = MotorcycleModel.Create("Yamaha MT-03");
        var newYear = ManufactureYear.Create(2021);
        var newLicensePlate = LicensePlate.Create("XYZ9W87");

        // Act
        var updatedDetails = details.Update(newModel, newYear, newLicensePlate);

        // Assert
        updatedDetails.Year().Should().Be(newYear);
        updatedDetails.Model().Should().Be(newModel);
        updatedDetails.LicensePlate().Should().Be(newLicensePlate);
        details.Year().Should().Be(_validYear);
    }
}
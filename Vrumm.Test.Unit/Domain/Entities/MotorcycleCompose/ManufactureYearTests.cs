using FluentAssertions;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.MotorcycleCompose;
public class ManufactureYearTests
{
    [Fact]
    public void Create_ValidYear_ReturnsInstance()
    {
        // Arrange
        var validYear = 2020;

        // Act
        var manufactureYear = ManufactureYear.Create(validYear);

        // Assert
        manufactureYear.ToInt().Should().Be(validYear);
        manufactureYear.ToString().Should().Be(validYear.ToString());
    }

    [Fact]
    public void Create_YearBefore1900_ThrowsDomainException()
    {
        // Arrange
        var invalidYear = 1899;

        // Act
        Action act = () => ManufactureYear.Create(invalidYear);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Ano da moto inválido");
    }

    [Fact]
    public void Create_YearAfterNextYear_ThrowsDomainException()
    {
        // Arrange
        var invalidYear = DateTime.Now.Year + 2;

        // Act
        Action act = () => ManufactureYear.Create(invalidYear);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Ano da moto inválido");
    }

    [Fact]
    public void Equals_SameYear_ReturnsTrue()
    {
        // Arrange
        var year1 = ManufactureYear.Create(2020);
        var year2 = ManufactureYear.Create(2020);

        // Act
        var areEqual = year1.Equals(year2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentYear_ReturnsFalse()
    {
        // Arrange
        var year1 = ManufactureYear.Create(2020);
        var year2 = ManufactureYear.Create(2021);

        // Act
        var areEqual = year1.Equals(year2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameYear_ReturnsSameHash()
    {
        // Arrange
        var year1 = ManufactureYear.Create(2020);
        var year2 = ManufactureYear.Create(2020);

        // Act
        var hash1 = year1.GetHashCode();
        var hash2 = year2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}
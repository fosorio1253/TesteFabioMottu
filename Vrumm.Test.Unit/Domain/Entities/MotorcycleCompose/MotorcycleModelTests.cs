using FluentAssertions;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities.MotorcycleCompose;
public class MotorcycleModelTests
{
    [Fact]
    public void Create_ValidModel_ReturnsInstance()
    {
        // Arrange
        var validModel = "Honda CB500";

        // Act
        var motorcycleModel = MotorcycleModel.Create(validModel);

        // Assert
        motorcycleModel.GetValue().Should().Be(validModel);
        motorcycleModel.ToStringRepresentation().Should().Be(validModel);
        motorcycleModel.ToString().Should().Be($"MotorcycleModel: {validModel}");
    }

    [Fact]
    public void Create_EmptyModel_ThrowsDomainException()
    {
        // Act
        Action act = () => MotorcycleModel.Create("");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Modelo da moto não pode estar vazio");
    }

    [Fact]
    public void Equals_SameModel_ReturnsTrue()
    {
        // Arrange
        var model1 = MotorcycleModel.Create("Honda CB500");
        var model2 = MotorcycleModel.Create("Honda CB500");

        // Act
        var areEqual = model1.Equals(model2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentModel_ReturnsFalse()
    {
        // Arrange
        var model1 = MotorcycleModel.Create("Honda CB500");
        var model2 = MotorcycleModel.Create("Yamaha MT-03");

        // Act
        var areEqual = model1.Equals(model2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameModel_ReturnsSameHash()
    {
        // Arrange
        var model1 = MotorcycleModel.Create("Honda CB500");
        var model2 = MotorcycleModel.Create("Honda CB500");

        // Act
        var hash1 = model1.GetHashCode();
        var hash2 = model2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}
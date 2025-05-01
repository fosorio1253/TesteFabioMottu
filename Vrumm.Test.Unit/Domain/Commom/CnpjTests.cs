using FluentAssertions;
using Vrumm.Domain.Common;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Commom;
public class CnpjTests
{
    [Fact]
    public void Create_ValidCnpj_ReturnsCnpjInstance()
    {
        // Arrange
        var validCnpj = "12345678000195"; // CNPJ válido com dígitos verificadores corretos

        // Act
        var cnpj = Cnpj.Create(validCnpj);

        // Assert
        cnpj.Value.Should().Be("12345678000195");
        cnpj.ToString().Should().Be("12345678000195");
    }

    [Fact]
    public void Create_EmptyCnpj_ThrowsDomainException()
    {
        // Arrange
        var emptyCnpj = "";

        // Act
        Action act = () => Cnpj.Create(emptyCnpj);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("CNPJ cannot be empty.");
    }

    [Fact]
    public void Create_InvalidLengthCnpj_ThrowsDomainException()
    {
        // Arrange
        var shortCnpj = "1234567890123"; // 13 dígitos

        // Act
        Action act = () => Cnpj.Create(shortCnpj);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("CNPJ must contain 14 digits.");
    }

    [Fact]
    public void Create_CnpjWithInvalidCheckDigits_ThrowsDomainException()
    {
        // Arrange
        var invalidCnpj = "12345678901234"; // Dígitos verificadores inválidos

        // Act
        Action act = () => Cnpj.Create(invalidCnpj);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("CNPJ has invalid check digits.");
    }

    [Fact]
    public void Create_CnpjWithNonDigits_StillValidatesCorrectly()
    {
        // Arrange
        var formattedCnpj = "12.345.678/0001-95"; // CNPJ válido com pontuação

        // Act
        var cnpj = Cnpj.Create(formattedCnpj);

        // Assert
        cnpj.Value.Should().Be("12345678000195"); // Deve normalizar
    }

    [Fact]
    public void Equals_SameCnpj_ReturnsTrue()
    {
        // Arrange
        var cnpj1 = Cnpj.Create("12345678000195");
        var cnpj2 = Cnpj.Create("12345678000195");

        // Act
        var areEqual = cnpj1.Equals(cnpj2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentCnpj_ReturnsFalse()
    {
        // Arrange
        var cnpj1 = Cnpj.Create("12345678000195");
        var cnpj2 = Cnpj.Create("98765432000188");

        // Act
        var areEqual = cnpj1.Equals(cnpj2);

        // Assert
        areEqual.Should().BeFalse();
    }
}
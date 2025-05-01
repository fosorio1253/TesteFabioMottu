using FluentAssertions;
using Vrumm.Domain.Common;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Commom;
public class BirthDateTests
{
    [Fact]
    public void Create_ValidBirthDate_ReturnsBirthDateInstance()
    {
        // Arrange
        var validDate = new DateTime(1995, 5, 15);

        // Act
        var birthDate = BirthDate.Create(validDate);

        // Assert
        birthDate.Value.Should().Be(validDate);
        birthDate.ToString().Should().Be("1995-05-15");
    }

    [Fact]
    public void Create_EmptyBirthDate_ThrowsDomainException()
    {
        // Arrange
        var emptyDate = default(DateTime);

        // Act
        Action act = () => BirthDate.Create(emptyDate);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Birth date cannot be empty.");
    }

    [Fact]
    public void Create_Under18YearsOld_ThrowsDomainException()
    {
        // Arrange
        var today = DateTime.Today;
        var under18Date = today.AddYears(-17).AddDays(1);

        // Act
        Action act = () => BirthDate.Create(under18Date);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Driver must be at least 18 years old.");
    }

    [Fact]
    public void Create_Before1900_ThrowsDomainException()
    {
        // Arrange
        var oldDate = new DateTime(1899, 12, 31);

        // Act
        Action act = () => BirthDate.Create(oldDate);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Birth date is invalid (too old).");
    }

    [Fact]
    public void Equals_SameDate_ReturnsTrue()
    {
        // Arrange
        var date1 = BirthDate.Create(new DateTime(1995, 5, 15));
        var date2 = BirthDate.Create(new DateTime(1995, 5, 15));

        // Act
        var areEqual = date1.Equals(date2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentDate_ReturnsFalse()
    {
        // Arrange
        var date1 = BirthDate.Create(new DateTime(1995, 5, 15));
        var date2 = BirthDate.Create(new DateTime(1995, 5, 16));

        // Act
        var areEqual = date1.Equals(date2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameDate_ReturnsSameHash()
    {
        // Arrange
        var date1 = BirthDate.Create(new DateTime(1995, 5, 15));
        var date2 = BirthDate.Create(new DateTime(1995, 5, 15));

        // Act
        var hash1 = date1.GetHashCode();
        var hash2 = date2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}
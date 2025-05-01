using FluentAssertions;
using Vrumm.Domain.Common;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Commom;
public class ValueObjectTests
{
    private class TestValueObject : ValueObject
    {
        private readonly string _value1;
        private readonly int _value2;

        public TestValueObject(string value1, int value2)
        {
            _value1 = value1;
            _value2 = value2;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value1;
            yield return _value2;
        }
    }

    [Fact]
    public void Equals_SameComponents_ReturnsTrue()
    {
        // Arrange
        var obj1 = new TestValueObject("test", 42);
        var obj2 = new TestValueObject("test", 42);

        // Act
        var areEqual = obj1.Equals(obj2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentComponents_ReturnsFalse()
    {
        // Arrange
        var obj1 = new TestValueObject("test", 42);
        var obj2 = new TestValueObject("different", 42);

        // Act
        var areEqual = obj1.Equals(obj2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        // Arrange
        var obj1 = new TestValueObject("test", 42);
        var obj2 = new AnotherTestValueObject("test", 42);

        // Act
        var areEqual = obj1.Equals(obj2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var obj = new TestValueObject("test", 42);

        // Act
        var areEqual = obj.Equals(null);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void OperatorEqual_SameComponents_ReturnsTrue()
    {
        // Arrange
        var obj1 = new TestValueObject("test", 42);
        var obj2 = new TestValueObject("test", 42);

        // Act
        var areEqual = obj1 == obj2;

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void OperatorNotEqual_DifferentComponents_ReturnsTrue()
    {
        // Arrange
        var obj1 = new TestValueObject("test", 42);
        var obj2 = new TestValueObject("different", 42);

        // Act
        var areNotEqual = obj1 != obj2;

        // Assert
        areNotEqual.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameComponents_ReturnsSameHash()
    {
        // Arrange
        var obj1 = new TestValueObject("test", 42);
        var obj2 = new TestValueObject("test", 42);

        // Act
        var hash1 = obj1.GetHashCode();
        var hash2 = obj2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    private class AnotherTestValueObject : ValueObject
    {
        private readonly string _value1;
        private readonly int _value2;

        public AnotherTestValueObject(string value1, int value2)
        {
            _value1 = value1;
            _value2 = value2;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value1;
            yield return _value2;
        }
    }
}
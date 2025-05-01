using FluentAssertions;
using Vrumm.Domain.Common;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Commom;
public class EntityTests
{
    private class TestEntity : Entity<int>
    {
        public TestEntity(int id)
        {
            Id = id;
        }
    }

    [Fact]
    public void Constructor_SetsCreationAndUpdateDates()
    {
        // Act
        var entity = new TestEntity(1);

        // Assert
        entity.Id.Should().Be(1);
        entity.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.UpdateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateModificationDate_UpdatesUpdateDate()
    {
        // Arrange
        var entity = new TestEntity(1);
        var initialUpdateDate = entity.UpdateDate;

        // Act
        Thread.Sleep(100); // Garantir diferença de tempo
        entity.UpdateModificationDate();

        // Assert
        entity.UpdateDate.Should().BeAfter(initialUpdateDate);
    }

    [Fact]
    public void Equals_SameIdAndType_ReturnsTrue()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(1);

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(2);

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new AnotherTestEntity(1);

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Equals_DefaultId_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity(default);
        var entity2 = new TestEntity(default);

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void OperatorEqual_SameEntity_ReturnsTrue()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(1);

        // Act
        var areEqual = entity1 == entity2;

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void OperatorNotEqual_DifferentEntity_ReturnsTrue()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(2);

        // Act
        var areNotEqual = entity1 != entity2;

        // Assert
        areNotEqual.Should().BeTrue();
    }

    private class AnotherTestEntity : Entity<int>
    {
        public AnotherTestEntity(int id)
        {
            Id = id;
        }
    }
}
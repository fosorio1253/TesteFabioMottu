using FluentAssertions;
using Vrumm.Domain.Entities;
using Xunit;

namespace Vrumm.Test.Unit.Domain.Entities;
public class UserTests
{
    [Fact]
    public void Constructor_ValidParameters_SetsProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "johndoe";
        var passwordHash = "hashedpassword";
        var email = "john.doe@example.com";
        var roles = new List<string> { "Admin", "Entregador" };

        // Act
        var user = new User(id, username, passwordHash, email, roles);

        // Assert
        user.Id.Should().Be(id);
        user.Username.Should().Be(username);
        user.PasswordHash.Should().Be(passwordHash);
        user.Email.Should().Be(email);
        user.Roles.Should().BeEquivalentTo(roles);
        user.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.LastLogin.Should().BeNull();
    }

    [Fact]
    public void UpdateLastLogin_SetsLastLoginToCurrentTime()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "johndoe",
            "hashedpassword",
            "john.doe@example.com",
            new List<string>());

        // Act
        user.UpdateLastLogin();

        // Assert
        user.LastLogin.Should().BeCloseTo
            (DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        // Arrange
        var user1 = new User(
            Guid.NewGuid(),
            "johndoe",
            "hashedpassword",
            "john.doe@example.com",
            new List<string>());
        var user2 = new User(
            user1.Id,
            "janedoe",
            "differentpassword",
            "jane.doe@example.com",
            new List<string>());

        // Act
        var areEqual = user1.Equals(user2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        // Arrange
        var user1 = new User(
            Guid.NewGuid(),
            "johndoe",
            "hashedpassword",
            "john.doe@example.com",
            new List<string>());
        var user2 = new User(
            Guid.NewGuid(),
            "johndoe",
            "hashedpassword",
            "john.doe@example.com",
            new List<string>());

        // Act
        var areEqual = user1.Equals(user2);

        // Assert
        areEqual.Should().BeFalse();
    }
}
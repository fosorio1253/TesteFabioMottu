using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Data.Repositories;
public class UserRepositoryTests
{
    private readonly VrummDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VrummDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new VrummDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = CreateUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task AddAsync_AddsUser()
    {
        // Arrange
        var user = CreateUser();

        // Act
        await _repository.AddAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync();
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("test@example.com");
    }

    private User CreateUser()
    {
        return new User(
            id: Guid.NewGuid(),
            username: "testuser",
            passwordHash: "hashedpassword",
            email: "test@example.com",
            roles: new List<string> { UserRole.Entregador }
        );
    }
}
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Auth.Commands.Register;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions;
using Xunit;

namespace Vrumm.Test.Unit.Application.Auth;
public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();
        _handler = new RegisterUserCommandHandler(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_RegistersUserAndReturnsId()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Username = "John",
            Email = "user@example.com",
            Password = "Password123"
        };

        command.Roles.Add(UserRole.Admin);

        var userId = command.Id;

        var user = new User(
            userId,
            command.Email,
            command.Username,
            command.Password,
            command.Roles);

        _userServiceMock.Setup(u => u.CreateUserAsync
        (user, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userId);
        _userServiceMock.Verify(u => u.CreateUserAsync
        (user, command.Password, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsDomainException()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Username = "John",
            Email = "user@example.com",
            Password = "Password123"
        };

        command.Roles.Add(UserRole.Admin);

        var userId = command.Id;

        var user = new User(
            userId,
            command.Email,
            command.Username,
            command.Password,
            command.Roles);

        _userServiceMock.Setup(u => u.CreateUserAsync
        (user, command.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DomainException("Email already exists"));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}
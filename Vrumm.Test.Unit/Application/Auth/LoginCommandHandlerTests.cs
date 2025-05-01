using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Auth.Commands.Login;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions.Auth;
using Xunit;

namespace Vrumm.Test.Unit.Application.Auth;
public class LoginCommandHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ITokenService> _jwtTokenServiceMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _jwtTokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();
        _handler = new LoginCommandHandler
            (_userServiceMock.Object, _jwtTokenServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthToken()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "user@example.com",
            Password = "Password123"
        };

        List<string> roles = new List<string>();

        roles.Add(UserRole.Admin);

        var user = new User(
            Guid.NewGuid(),
            command.Username,
            command.Password,
            "Jack",
            roles);

        var token = "jwt-token";

        _userServiceMock.Setup(u => u.CheckPasswordAsync
        (user, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _jwtTokenServiceMock.Setup(j => j.GenerateTokenAsync
        (user, CancellationToken.None))
            .Returns(Task.FromResult(token));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(token);
        
        _userServiceMock.Verify(u => u.CheckPasswordAsync
        (user, command.Password, It.IsAny<CancellationToken>()), Times.Once());
        
        _jwtTokenServiceMock.Verify(j => j.GenerateTokenAsync
        (user, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ThrowsAuthenticationException()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "user@example.com",
            Password = "WrongPassword"
        };
        List<string> roles = new List<string>();

        roles.Add(UserRole.Admin);

        var user = new User(
            Guid.NewGuid(),
            command.Username,
            command.Password,
            "Jack",
            roles);

        _userServiceMock.Setup(u => u.CheckPasswordAsync
        (user, command.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthenticationException("Invalid credentials"));

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}
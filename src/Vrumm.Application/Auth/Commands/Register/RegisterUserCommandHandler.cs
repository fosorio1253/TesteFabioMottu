using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;

namespace Vrumm.Application.Auth.Commands.Register;
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserService userService,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var methodName = nameof(RegisterUserCommandHandler);
        _logger.LogInformation("[{MethodName}] Registering new user: {Username}", methodName, command.Username);

        var existingUser = await _userService.GetByUsernameAsync(command.Username, cancellationToken);
        if (existingUser != null)
        {
            _logger.LogWarning("[{MethodName}] Username already taken: {Username}", methodName, command.Username);
            throw new ValidationException("Username already taken");
        }

        var user = new User(
            command.Id,
            command.Username,
            string.Empty,
            command.Email,
            command.Roles);

        var createdUser = await _userService.CreateUserAsync(user, command.Password, cancellationToken);

        _logger.LogInformation("[{MethodName}] User registered successfully: {Username}, Id: {UserId}",
            methodName, command.Username, createdUser.Id);

        return createdUser.Id;
    }
}
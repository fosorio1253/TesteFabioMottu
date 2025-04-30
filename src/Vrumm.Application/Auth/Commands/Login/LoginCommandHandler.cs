using Microsoft.Extensions.Logging;
using System.Security.Authentication;
using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Auth.Commands.Login;
public class LoginCommandHandler : ICommandHandler<LoginCommand, string>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserService userService,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var methodName = nameof(LoginCommandHandler);
        _logger.LogInformation("[{MethodName}] Attempting login for user: {Username}", methodName, command.Username);

        var user = await _userService.GetByUsernameAsync(command.Username, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("[{MethodName}] User not found: {Username}", methodName, command.Username);
            throw new AuthenticationException("Invalid username or password");
        }

        var isPasswordValid = await _userService.CheckPasswordAsync(user, command.Password, cancellationToken);
        if (!isPasswordValid)
        {
            _logger.LogWarning("[{MethodName}] Invalid password for user: {Username}", methodName, command.Username);
            throw new AuthenticationException("Invalid username or password");
        }

        user.UpdateLastLogin();
        var token = await _tokenService.GenerateTokenAsync(user, cancellationToken);

        _logger.LogInformation("[{MethodName}] User authenticated successfully: {Username}", methodName, command.Username);
        return token;
    }
}
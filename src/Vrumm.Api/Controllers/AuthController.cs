using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vrumm.Api.Models;
using Vrumm.Api.Models.Auth;
using Vrumm.Application.Auth.Commands.Login;
using Vrumm.Application.Auth.Commands.Register;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;

namespace Vrumm.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AuthController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher
            ?? throw new ArgumentNullException(nameof(commandDispatcher));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand
        {
            Username = request.Username,
            Password = request.Password
        };

        var token = await _commandDispatcher
            .DispatchAsync<LoginCommand, string>(command, cancellationToken);

        return Ok(new LoginResponse { Token = token });
    }

    [HttpPost("register")]
    [Authorize(Roles = UserRole.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Password = request.Password,
            Email = request.Email,
            Roles = request.Roles
        };

        var userId = await _commandDispatcher
            .DispatchAsync<RegisterUserCommand, Guid>(command, cancellationToken);

        return CreatedAtAction(nameof(Register), new { id = userId }, null);
    }
}
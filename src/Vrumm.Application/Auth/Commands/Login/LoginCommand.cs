using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Auth.Commands.Login;
public class LoginCommand : ICommand<string>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
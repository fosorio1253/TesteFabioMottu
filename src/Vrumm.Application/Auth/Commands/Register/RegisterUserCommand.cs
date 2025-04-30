using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Auth.Commands.Register;
public class RegisterUserCommand : ICommand<Guid>
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}
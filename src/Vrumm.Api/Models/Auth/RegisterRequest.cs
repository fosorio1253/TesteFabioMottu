using System.ComponentModel.DataAnnotations;

namespace Vrumm.Api.Models.Auth;
public class RegisterRequest
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password é obrigatório")]
    [MinLength(8, ErrorMessage = "Password deve ter pelo menos 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Roles são obrigatórias")]
    public List<string> Roles { get; set; } = new List<string>();
}
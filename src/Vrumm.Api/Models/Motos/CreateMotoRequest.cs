using System.ComponentModel.DataAnnotations;

namespace Vrumm.Api.Models.Motos;
public class CreateMotoRequest
{
    [Required(ErrorMessage = "Identificador é obrigatório")]
    [StringLength(50, ErrorMessage = "Identificador não pode exceder 50 caracteres")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ano é obrigatório")]
    [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100")]
    public int Ano { get; set; }

    [Required(ErrorMessage = "Modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "Modelo não pode exceder 100 caracteres")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Placa é obrigatória")]
    [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "Placa deve seguir o formato ABC-1234")]
    public string Placa { get; set; } = string.Empty;
}
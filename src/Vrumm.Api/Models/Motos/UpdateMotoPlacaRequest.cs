using System.ComponentModel.DataAnnotations;

namespace Vrumm.Api.Models.Motos;
public class UpdateMotoPlacaRequest
{
    [Required(ErrorMessage = "Placa é obrigatória")]
    [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "Placa deve seguir o formato ABC-1234")]
    public string Placa { get; set; } = string.Empty;
}
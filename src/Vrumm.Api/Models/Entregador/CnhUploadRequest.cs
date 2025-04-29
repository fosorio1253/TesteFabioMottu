using System.ComponentModel.DataAnnotations;

namespace Vrumm.Api.Models.Entregador;
public class CnhUploadRequest
{
    [Required(ErrorMessage = "Imagem da CNH é obrigatória")]
    public string ImagemCnh { get; set; } = string.Empty;
}
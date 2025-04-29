using System.ComponentModel.DataAnnotations;

namespace Vrumm.Api.Models.Locacao;
public class CreateLocacaoRequest
{
    [Required(ErrorMessage = "Entregador ID é obrigatório")]
    [StringLength(50, ErrorMessage = "Entregador ID não pode exceder 50 caracteres")]
    public string EntregadorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Moto ID é obrigatório")]
    [StringLength(50, ErrorMessage = "Moto ID não pode exceder 50 caracteres")]
    public string MotoId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data de início é obrigatória")]
    public DateTime DataInicio { get; set; }

    [Required(ErrorMessage = "Data de término é obrigatória")]
    public DateTime DataTermino { get; set; }

    [Required(ErrorMessage = "Data de previsão de término é obrigatória")]
    public DateTime DataPrevisaoTermino { get; set; }

    [Required(ErrorMessage = "Plano é obrigatório")]
    [Range(7, 50, ErrorMessage = "Plano deve ser 7, 15, 30, 45 ou 50")]
    public int Plano { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Vrumm.Api.Models.Locacao;
public class LocacaoDevolucaoRequest
{
    [Required(ErrorMessage = "Data de devolução é obrigatória")]
    public DateTime DataDevolucao { get; set; }
}
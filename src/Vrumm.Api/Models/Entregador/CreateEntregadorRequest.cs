using System.ComponentModel.DataAnnotations;

namespace Vrumm.Api.Models.Entregador;
public class CreateEntregadorRequest
{
    [Required(ErrorMessage = "Identificador é obrigatório")]
    [StringLength(50, ErrorMessage = "Identificador não pode exceder 50 caracteres")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve ter 14 dígitos")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "Número da CNH é obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Número da CNH deve ter 11 dígitos")]
    public string NumeroCnh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo de CNH é obrigatório")]
    [RegularExpression(@"^(A|B|A\+B)$", ErrorMessage = "Tipo de CNH deve ser A, B ou A+B")]
    public string TipoCnh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Imagem da CNH é obrigatória")]
    public string ImagemCnh { get; set; } = string.Empty;
}
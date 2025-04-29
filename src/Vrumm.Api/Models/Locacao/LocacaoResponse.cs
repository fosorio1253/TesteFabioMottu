namespace Vrumm.Api.Models.Locacao;
public class LocacaoResponse
{
    public string Identificador { get; set; } = string.Empty;
    public decimal ValorDiaria { get; set; }
    public string EntregadorId { get; set; } = string.Empty;
    public string MotoId { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataTermino { get; set; }
    public DateTime DataPrevisaoTermino { get; set; }
    public DateTime? DataDevolucao { get; set; }
}
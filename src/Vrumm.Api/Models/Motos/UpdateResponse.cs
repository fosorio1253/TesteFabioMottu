namespace Vrumm.Api.Models.Motos;
public class UpdateResponse
{
    public string Mensagem { get; set; } = string.Empty;

    public UpdateResponse(string mensagem)
    {
        Mensagem = mensagem;
    }
}
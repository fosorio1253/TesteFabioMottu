namespace Vrumm.Api.Models;
public class ErrorResponse
{
    public string Mensagem { get; }

    public ErrorResponse(string mensagem)
    {
        Mensagem = mensagem;
    }
}
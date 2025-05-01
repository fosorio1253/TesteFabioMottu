using FluentAssertions;
using System.Net.Http.Json;
using System.Net;
using Vrumm.Test.Integration.Auth;
using Vrumm.Test.Integration.common;
using Xunit;

namespace Vrumm.Test.Integration.Locacao;
public class LocacaoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LocacaoControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateLocacao_Should_Return_Created_When_Valid()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");

        var request = new
        {
            entregadorId = Guid.NewGuid().ToString(),
            motoId = Guid.NewGuid().ToString(),
            dataInicio = DateTime.UtcNow,
            dataTermino = DateTime.UtcNow.AddDays(7),
            dataPrevisaoTermino = DateTime.UtcNow.AddDays(7),
            plano = 7
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/locacao");
        httpRequest.Headers.Authorization
            = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        httpRequest.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(httpRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetLocacaoById_Should_Return_NotFound_When_Locacao_Not_Exists()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var request = new HttpRequestMessage(HttpMethod.Get, $"/locacao/{Guid.NewGuid()}");
        request.Headers.Authorization
            = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FinalizeLocacao_Should_Return_Ok_When_Valid()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var rentalId = Guid.NewGuid();

        var create = new HttpRequestMessage(HttpMethod.Post, "/locacao")
        {
            Headers = { Authorization = new("Bearer", token) },
            Content = JsonContent.Create(new
            {
                entregadorId = Guid.NewGuid().ToString(),
                motoId = Guid.NewGuid().ToString(),
                dataInicio = DateTime.UtcNow,
                dataTermino = DateTime.UtcNow.AddDays(7),
                dataPrevisaoTermino = DateTime.UtcNow.AddDays(7),
                plano = 7
            })
        };
        await _client.SendAsync(create);

        var finalize = new HttpRequestMessage(HttpMethod.Put, $"/locacao/{rentalId}/devolucao")
        {
            Headers = { Authorization = new("Bearer", token) },
            Content = JsonContent.Create(new
            {
                dataDevolucao = DateTime.UtcNow.AddDays(5)
            })
        };

        var response = await _client.SendAsync(finalize);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
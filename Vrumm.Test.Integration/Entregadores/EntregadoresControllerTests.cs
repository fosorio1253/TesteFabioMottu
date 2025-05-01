using FluentAssertions;
using System.Net.Http.Json;
using System.Net;
using Vrumm.Test.Integration.common;
using Xunit;
using Vrumm.Test.Integration.Auth;

namespace Vrumm.Test.Integration.Entregadores;
public class EntregadoresControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EntregadoresControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateEntregador_Should_Return_Created_When_Valid()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var request = new
        {
            identificador = Guid.NewGuid().ToString(),
            nome = "João da Silva",
            cnpj = "12345678000199",
            dataNascimento = DateTime.UtcNow.AddYears(-25),
            numeroCnh = "12345678901",
            tipoCnh = "A",
            imagemCnh = Convert.ToBase64String(new byte[] { 1, 2, 3 })
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/entregadores");
        httpRequest.Headers.Authorization
            = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        httpRequest.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(httpRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UploadCnh_Should_Return_Created_When_Valid()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var driverId = Guid.NewGuid();

        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/entregadores")
        {
            Headers = { Authorization = new("Bearer", token) },
            Content = JsonContent.Create(new
            {
                identificador = driverId.ToString(),
                nome = "José da Silva",
                cnpj = "12345678901234",
                dataNascimento = DateTime.UtcNow.AddYears(-30),
                numeroCnh = "12345678901",
                tipoCnh = "A",
                imagemCnh = Convert.ToBase64String(new byte[] { 1, 2, 3 })
            })
        });

        var uploadRequest = new HttpRequestMessage
            (HttpMethod.Post, $"/entregadores/{driverId}/cnh")
        {
            Headers = { Authorization = new("Bearer", token) },
            Content = JsonContent.Create(new
            {
                imagemCnh = Convert.ToBase64String(new byte[] { 4, 5, 6 })
            })
        };

        var response = await _client.SendAsync(uploadRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
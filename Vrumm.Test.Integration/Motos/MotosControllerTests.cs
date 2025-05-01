using FluentAssertions;
using System.Net.Http.Json;
using System.Net;
using Vrumm.Test.Integration.Auth;
using Vrumm.Test.Integration.common;
using Xunit;

namespace Vrumm.Test.Integration.Motos;
public class MotosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MotosControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateMoto_Should_Return_Created()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var request = new
        {
            identificador = Guid.NewGuid().ToString(),
            ano = 2023,
            modelo = "CG 160",
            placa = "ABC-1234"
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/motos");
        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        httpRequest.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(httpRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetMotoById_Should_Return_Ok_When_Found()
    {
        // Arrange
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var id = Guid.NewGuid();

        var createRequest = new
        {
            identificador = id.ToString(),
            ano = 2023,
            modelo = "XRE 300",
            placa = "DEF-5678"
        };

        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/motos");
        postRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        postRequest.Content = JsonContent.Create(createRequest);
        await _client.SendAsync(postRequest);

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/motos/{id}");
        getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(getRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMotoById_Should_Return_NotFound_When_NotExists()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/motos/{Guid.NewGuid()}");
        getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(getRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateMotoPlaca_Should_Return_Ok_When_Valid()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var id = Guid.NewGuid();

        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/motos")
        {
            Headers = { Authorization = new("Bearer", token) },
            Content = JsonContent.Create(new
            {
                identificador = id.ToString(),
                ano = 2023,
                modelo = "NXR 160",
                placa = "GHI-9012"
            })
        });

        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/motos/{id}/placa")
        {
            Headers = { Authorization = new("Bearer", token) },
            Content = JsonContent.Create(new { placa = "JKL-3456" })
        };

        var response = await _client.SendAsync(updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteMoto_Should_Return_Ok_When_Successful()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var id = Guid.NewGuid();

        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/motos")
        {
            Headers = { Authorization = new("Bearer", token) },
            Content = JsonContent.Create(new
            {
                identificador = id.ToString(),
                ano = 2023,
                modelo = "CB 500",
                placa = "MNO-6789"
            })
        });

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/motos/{id}")
        {
            Headers = { Authorization = new("Bearer", token) }
        };

        var response = await _client.SendAsync(deleteRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMotos_Should_Return_Ok_List()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var request = new HttpRequestMessage(HttpMethod.Get, "/motos");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
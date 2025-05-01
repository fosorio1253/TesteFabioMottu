using System.Net.Http.Json;
using System.Net;
using Vrumm.Test.Integration.common;
using Xunit;
using FluentAssertions;

namespace Vrumm.Test.Integration.Auth;
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var credentials = new { username = "admin", password = "Admin123!" };
        var response = await _client.PostAsJsonAsync("/auth/login", credentials);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_Should_Return_BadRequest_When_Credentials_Are_Invalid()
    {
        var credentials = new { username = "admin", password = "WrongPassword" };
        var response = await _client.PostAsJsonAsync("/auth/login", credentials);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_Should_Create_User_When_Admin_Token_Provided()
    {
        var token = await AuthHelper.GetTokenAsync(_client, "admin", "Admin123!");
        var request = new
        {
            username = "newuser",
            password = "NewUser123!",
            email = "newuser@example.com",
            roles = new[] { "Entregador" }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/auth/register");
        httpRequest.Headers.Authorization
            = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        httpRequest.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(httpRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_Should_Return_Forbidden_When_Token_Is_Missing()
    {
        var request = new
        {
            username = "unauthorized",
            password = "User123!",
            email = "unauth@example.com",
            roles = new[] { "Entregador" }
        };

        var response = await _client.PostAsJsonAsync("/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMotoById_Should_Return_Unauthorized_Without_Token()
    {
        var response = await _client.GetAsync($"/motos/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateMoto_Should_Return_Forbidden_Without_Admin_Role()
    {
        var fakeToken = "ey.fake.token";
        var request = new HttpRequestMessage(HttpMethod.Post, "/motos")
        {
            Headers = { Authorization = new("Bearer", fakeToken) },
            Content = JsonContent.Create(new
            {
                identificador = Guid.NewGuid().ToString(),
                ano = 2023,
                modelo = "CB 300",
                placa = "PQR-4567"
            })
        };

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
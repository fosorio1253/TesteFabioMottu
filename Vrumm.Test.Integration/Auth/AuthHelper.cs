using System.Net.Http.Json;
using System.Text.Json;

namespace Vrumm.Test.Integration.Auth;
public static class AuthHelper
{
    public static async Task<string> GetTokenAsync
        (HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new
        {
            username,
            password
        });

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Token retrieval failed.");

        var json = await response.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return parsed!.Token;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
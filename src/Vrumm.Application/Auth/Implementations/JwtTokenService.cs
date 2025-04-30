using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;

namespace Vrumm.Application.Auth.Implementations;
public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IConfiguration configuration,
        ILogger<JwtTokenService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var methodName = nameof(GenerateTokenAsync);
        _logger.LogInformation("[{MethodName}] Generating token for user: {UserId}", methodName, user.Id);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = GetUserClaims(user);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiryInHours"])),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogInformation("[{MethodName}] Token generated successfully for user: {UserId}", methodName, user.Id);
        return Task.FromResult(tokenString);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var methodName = nameof(ValidateToken);
        _logger.LogInformation("[{MethodName}] Validating token", methodName);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            _logger.LogInformation("[{MethodName}] Token validated successfully", methodName);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("[{MethodName}] Token validation failed: {ErrorMessage}", methodName, ex.Message);
            throw;
        }
    }

    public IEnumerable<Claim> GetUserClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
        };
    }
}
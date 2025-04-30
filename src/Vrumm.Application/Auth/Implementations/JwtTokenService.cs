using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Options;

namespace Vrumm.Application.Auth.Implementations;
public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IOptions<JwtOptions> jwtOptions,
        ILogger<JwtTokenService> logger)
    {
        _jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var methodName = nameof(GenerateTokenAsync);
        _logger.LogInformation("[{MethodName}] Generating token for user: {UserId}", methodName, user.Id);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = GetUserClaims(user);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiryInHours),
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
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key))
        };
    }
}
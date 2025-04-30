using System.Security.Claims;
using Vrumm.Domain.Entities;

namespace Vrumm.Application.Common.Interfaces;
public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken);
    ClaimsPrincipal ValidateToken(string token);
    IEnumerable<Claim> GetUserClaims(User user);
}

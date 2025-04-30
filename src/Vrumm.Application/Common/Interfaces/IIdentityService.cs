namespace Vrumm.Application.Common.Interfaces;
public interface IIdentityService
{
    bool IsInRole(string role);
    Dictionary<string, string> GetCurrentUserClaims();
    Guid GetCurrentUserId();
}

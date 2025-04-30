using Vrumm.Domain.Entities;

namespace Vrumm.Application.Common.Interfaces;
public interface IUserService
{
    Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken);
    Task<bool> CheckPasswordAsync(User user, string password, CancellationToken cancellationToken);
}
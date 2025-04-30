using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Auth.Implementations;
public class UserService : IUserService
{
    private readonly IUnitOfWork _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork dbContext,
        IPasswordHasher<User> passwordHasher,
        ILogger<UserService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var methodName = nameof(GetByIdAsync);
        _logger.LogInformation("[{MethodName}] Getting user by id: {UserId}", methodName, id);

        return await _dbContext.Users.GetByIdAsync(id , cancellationToken);
    }

    public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var methodName = nameof(GetByUsernameAsync);
        _logger.LogInformation("[{MethodName}] Getting user by username: {Username}", methodName, username);

        var queryUser = await _dbContext.Users.GetQueryAsync();
        var user = queryUser.FirstOrDefault(u 
            => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        return user;
    }

    public async Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken)
    {
        var methodName = nameof(CreateUserAsync);
        _logger.LogInformation("[{MethodName}] Creating new user: {Username}", methodName, user.Username);

        var passwordHash = _passwordHasher.HashPassword(user, password);

        var newUser = new User(
            user.Id,
            user.Username,
            passwordHash,
            user.Email,
            user.Roles);

        await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[{MethodName}] User created successfully: {UserId}", methodName, newUser.Id);
        return newUser;
    }

    public Task<bool> CheckPasswordAsync(User user, string password, CancellationToken cancellationToken)
    {
        var methodName = nameof(CheckPasswordAsync);
        _logger.LogInformation("[{MethodName}] Checking password for user: {UserId}", methodName, user.Id);

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return Task.FromResult(result == PasswordVerificationResult.Success);
    }
}

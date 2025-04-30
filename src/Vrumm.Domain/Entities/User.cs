namespace Vrumm.Domain.Entities;
public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string Email { get; private set; }
    public List<string> Roles { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLogin { get; private set; }

    private User() { }

    public User(Guid id, string username, string passwordHash, string email, List<string> roles)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
        Roles = roles;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
    }
}
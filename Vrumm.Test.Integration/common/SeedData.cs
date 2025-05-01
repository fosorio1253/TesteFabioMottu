using Microsoft.AspNetCore.Identity;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Test.Integration.common;
public static class SeedData
{
    public static void Initialize(VrummDbContext context)
    {
        if (context.Users.Any()) return;

        var hasher = new PasswordHasher<User>();
        var user = new User(
            Guid.NewGuid(),
            "admin",
            hasher.HashPassword(null!, "Admin123!"),
            "admin@example.com",
            new List<string> { "Admin" });

        context.Users.Add(user);
        context.SaveChanges();
    }
}
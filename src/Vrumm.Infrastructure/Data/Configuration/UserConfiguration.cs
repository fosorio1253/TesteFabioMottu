using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vrumm.Domain.Entities;

namespace Vrumm.Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Username).IsRequired().HasMaxLength(50);
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.Property(e => e.Email).IsRequired().HasMaxLength(100);

        // Configure the Roles property to be stored as a JSON array
        builder.Property(e => e.Roles).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
}

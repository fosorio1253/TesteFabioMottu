using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Entities.DriverCompose;

namespace Vrumm.Infrastructure.Data.Context;
public class VrummDbContext : DbContext
{
    public DbSet<Motorcycle> Motorcycles { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<MotorcycleRegistrationEvent> MotorcycleRegistrationEvents { get; set; }

    public VrummDbContext(DbContextOptions<VrummDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not null)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entry.Entity.GetType().GetProperty("CreationDate") != null)
                            entry.Property("CreationDate").CurrentValue = now;
                        if (entry.Entity.GetType().GetProperty("UpdateDate") != null)
                            entry.Property("UpdateDate").CurrentValue = now;
                        break;
                    case EntityState.Modified:
                        if (entry.Entity.GetType().GetProperty("UpdateDate") != null)
                            entry.Property("UpdateDate").CurrentValue = now;
                        break;
                }
            }
        }
    }
}
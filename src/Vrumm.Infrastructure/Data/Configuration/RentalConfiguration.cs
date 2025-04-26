using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities;

namespace Vrumm.Infrastructure.Data.Configuration;
public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("Rentals");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.MotorcycleId)
            .IsRequired();

        builder.Property(r => r.DriverId)
            .IsRequired();

        builder.Property(r => r.PlanId)
            .IsRequired();

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.ExpectedEndDate)
            .IsRequired();

        builder.Property(r => r.EndDate);

        builder.Property(r => r.TotalValue)
            .HasColumnType("decimal(10, 2)");

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.CreationDate)
            .IsRequired();

        builder.Property(r => r.UpdateDate)
            .IsRequired();

        builder.HasOne<Motorcycle>()
            .WithMany()
            .HasForeignKey(r => r.MotorcycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Driver>()
            .WithMany()
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Plan>()
            .WithMany()
            .HasForeignKey(r => r.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
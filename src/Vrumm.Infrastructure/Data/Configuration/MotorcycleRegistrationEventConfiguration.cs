using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities.MotorcycleCompose;

namespace Vrumm.Infrastructure.Data.Configuration;
public class MotorcycleRegistrationEventConfiguration : IEntityTypeConfiguration<MotorcycleRegistrationEvent>
{
    public void Configure(EntityTypeBuilder<MotorcycleRegistrationEvent> builder)
    {
        builder.ToTable("MotorcycleRegistrationEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.MotorcycleId)
            .IsRequired();

        builder.Property(e => e.Year)
            .IsRequired();

        builder.Property(e => e.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LicensePlate)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(e => e.EventTimestamp)
            .IsRequired();

        builder.Property(e => e.ProcessedAt)
            .IsRequired();

        builder.Property(e => e.CreationDate)
            .IsRequired();

        builder.Property(e => e.UpdateDate)
            .IsRequired();

        builder.HasIndex(e => e.MotorcycleId);

        builder.HasIndex(e => e.Year);
    }
}
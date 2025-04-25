using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vrumm.Domain.Entities;

namespace Vrumm.Infrastructure.Data.Configuration;
public class MotorcycleConfiguration : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        builder.ToTable("Motorcycles");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Year)
            .IsRequired();

        builder.Property(m => m.LicensePlate)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.CreationDate)
            .IsRequired();

        builder.Property(m => m.UpdateDate)
            .IsRequired();

        builder.HasIndex(m => m.LicensePlate)
            .IsUnique();
    }
}
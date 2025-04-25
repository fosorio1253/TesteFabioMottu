using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities;

namespace Vrumm.Infrastructure.Data.Configuration;
public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.TaxId)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(d => d.BirthDate)
            .IsRequired();

        builder.Property(d => d.LicenseNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.LicenseType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.LicenseImagePath)
            .HasMaxLength(255);

        builder.Property(d => d.CreationDate)
            .IsRequired();

        builder.Property(d => d.UpdateDate)
            .IsRequired();

        builder.HasIndex(d => d.TaxId)
            .IsUnique();

        builder.HasIndex(d => d.LicenseNumber)
            .IsUnique();
    }
}
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;

namespace Vrumm.Infrastructure.Data.Configuration;
public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Cnpj)
            .HasConversion(cnpj => cnpj.Value, value => Cnpj.Create(value))
            .IsRequired();

        builder.Property(d => d.BirthDate)
            .HasConversion(birthDate => birthDate.Value, value => BirthDate.Create(value))
            .IsRequired();

        builder.Property(d => d.LicenseNumber)
            .HasConversion(licenseNumber => licenseNumber.ToStringRepresentation(), value => LicenseNumber.Create(value))
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.LicenseType)
            .HasConversion(licenseType => licenseType.Value, value => LicenseTypeValue.Create(value.ToString()))
            .IsRequired();

        builder.Property(d => d.LicenseImagePath)
            .HasMaxLength(500);
    }
}
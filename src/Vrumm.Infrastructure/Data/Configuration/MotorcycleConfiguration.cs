using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.MotorcycleCompose;

namespace Vrumm.Infrastructure.Data.Configuration;
public class MotorcycleConfiguration : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        builder.HasKey(m => m.Id);

        builder.OwnsOne(m => m.Details(), details =>
        {
            details.Property(d => d.Year().ToInt()).HasColumnName("Year");
            details.Property(d => d.Model().ToStringRepresentation()).HasColumnName("Model");
            details.Property(d => d.LicensePlate().ToStringRepresentation()).HasColumnName("LicensePlate");
        });

        builder.Property(m => m.Status().ToStatus()).HasColumnName("Status")
              .HasConversion(
                  status => status,
                  value => MotorcycleStatusState.Available().ToStatus());

        builder.Property(m => m.CreationDate).HasColumnName("CreatedAt");
        builder.Property(m => m.UpdateDate).HasColumnName("ModifiedAt");
    }
}
using Vrumm.Application.Drivers.Dtos;
using Vrumm.Domain.Entities.DriverCompose;

namespace Vrumm.Application.Drivers.Mappings;
public static class DriverMapper
{
    public static DriverDto ToDto(Driver driver)
    {
        if (driver == null) throw new ArgumentNullException(nameof(driver));

        return new DriverDto
        {
            Id = driver.Id,
            Name = driver.Name,
            TaxId = driver.Cnpj.Value,
            BirthDate = driver.BirthDate.Value,
            LicenseNumber = driver.LicenseNumber.ToStringRepresentation(),
            LicenseType = driver.LicenseType.ToString(),
            LicenseImagePath = driver.LicenseImagePath,
            CreationDate = driver.CreationDate,
            UpdateDate = driver.UpdateDate
        };
    }
}
using Vrumm.Domain.Common;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.DriverCompose;
public class Driver : Entity<Guid>
{
    public string Name { get; private set; }
    public Cnpj Cnpj { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public LicenseNumber LicenseNumber { get; private set; }
    public LicenseTypeValue LicenseType { get; private set; }
    public string LicenseImagePath { get; private set; }

    private Driver() { }

    public Driver(string name, Cnpj cnpj, BirthDate birthDate, LicenseNumber licenseNumber, 
                  LicenseTypeValue licenseType) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Driver name cannot be empty.");

        Id = Guid.NewGuid();
        Name = name;
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        BirthDate = birthDate ?? throw new ArgumentNullException(nameof(birthDate));
        LicenseNumber = licenseNumber ?? throw new ArgumentNullException(nameof(licenseNumber));
        LicenseType = licenseType ?? throw new ArgumentNullException(nameof(licenseType));
    }

    public void Update(string name, Cnpj cnpj, BirthDate birthDate, LicenseNumber licenseNumber, 
                      LicenseTypeValue licenseType)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Driver name cannot be empty.");

        Name = name;
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        BirthDate = birthDate ?? throw new ArgumentNullException(nameof(birthDate));
        LicenseNumber = licenseNumber ?? throw new ArgumentNullException(nameof(licenseNumber));
        LicenseType = licenseType ?? throw new ArgumentNullException(nameof(licenseType));
        UpdateModificationDate();
    }

    public void UpdateLicenseImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new DomainException("License image path cannot be empty.");

        LicenseImagePath = imagePath;
        UpdateModificationDate();
    }

    public bool CanRentMotorcycle()
    {
        return  LicenseType.Value == Common.Enums.LicenseType.A
            || LicenseType.Value == Common.Enums.LicenseType.AB;
    }
}
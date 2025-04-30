using Vrumm.Domain.Common;
using Vrumm.Domain.Exceptions;
using Vrumm.Domain.Exceptions.Drivers;

namespace Vrumm.Domain.Entities.DriverCompose;
public class Driver : Entity<Guid>
{
    public string Name { get; private set; }
    public Cnpj Cnpj { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public LicenseNumber LicenseNumber { get; private set; }
    public LicenseTypeValue LicenseType { get; private set; }
    public string LicenseImagePath { get; private set; }
    public byte[] ImageBytes { get; private set; }
    public string ContentType { get; private set; }
    public string FileName { get; private set; }


    private Driver() { }

    public Driver(string name, Cnpj cnpj, BirthDate birthDate, LicenseNumber licenseNumber, 
                  LicenseTypeValue licenseType, string licenseImageBase64) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Driver name cannot be empty.");
        if (string.IsNullOrEmpty(licenseImageBase64))
            throw new DomainException("Imagem da CNH é obrigatória");

        Id = Guid.NewGuid();
        Name = name;
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        BirthDate = birthDate ?? throw new ArgumentNullException(nameof(birthDate));
        LicenseNumber = licenseNumber ?? throw new ArgumentNullException(nameof(licenseNumber));
        LicenseType = licenseType ?? throw new ArgumentNullException(nameof(licenseType));
        ImageBase64ToBytes(licenseImageBase64);
        SetContentType();
        SetFileName();
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

    private void ImageBase64ToBytes(string licenseImageBase64)
    {
        try
        {
            ImageBytes = Convert.FromBase64String(licenseImageBase64);
        }
        catch (FormatException)
        {
            throw new DomainException("Imagem da CNH deve ser um base64 válido");
        }
    }

    private void SetContentType()
    {
        ContentType = ImageBytes.Length > 0 && ImageBytes[1] == 0x50 ? "image/png" : "image/bmp";
        if (ContentType != "image/png" && ContentType != "image/bmp")
            throw new InvalidFileTypeException("Imagem da CNH deve ser PNG ou BMP");
    }

    private void SetFileName()
    {
        FileName = $"licenses/{Id}_cnh.{(ContentType == "image/png" ? "png" : "bmp")}";
    }
}
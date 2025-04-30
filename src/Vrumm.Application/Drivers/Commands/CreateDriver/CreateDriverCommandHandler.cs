using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions;
using Vrumm.Domain.Options;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Application.Drivers.Commands.CreateDriver;
public class CreateDriverCommandHandler : ICommandHandler<CreateDriverCommand, Guid>
{
    private readonly string _bucketName;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly ILogger<CreateDriverCommandHandler> _logger;

    public CreateDriverCommandHandler(
        IOptions<GoogleCloudStorageOptions> storageOptions,
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        ILogger<CreateDriverCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bucketName = storageOptions.Value?.LicenseBucketName
            ?? throw new InvalidOperationException("Google Cloud Storage bucket name not configured.");
    }

    public async Task<Guid> Handle(CreateDriverCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating driver with CNPJ {Cnpj}", command.Cnpj);

        var cnpj = Cnpj.Create(command.Cnpj);
        var licenseNumber = LicenseNumber.Create(command.LicenseNumber);

        if (await _unitOfWork.Drivers.ExistsByCnpjAsync(cnpj, cancellationToken))
        {
            _logger.LogWarning("Driver with CNPJ {Cnpj} already exists", cnpj.Value);
            throw new DuplicateCnpjException(cnpj.Value);
        }

        if (await _unitOfWork.Drivers.ExistsByLicenseNumberAsync(licenseNumber, cancellationToken))
        {
            _logger.LogWarning("Driver with LicenseNumber {LicenseNumber} already exists", licenseNumber.ToStringRepresentation());
            throw new DomainException("License number must be unique.");
        }

        var driver = new Driver(
            command.Name,
            cnpj,
            BirthDate.Create(command.BirthDate),
            licenseNumber,
            LicenseTypeValue.Create(command.LicenseType));

        if (string.IsNullOrEmpty(command.LicenseImageBase64))
        {
            _logger.LogWarning("License image is required for driver with CNPJ {Cnpj}", command.Cnpj);
            throw new DomainException("Imagem da CNH é obrigatória");
        }

        byte[] imageBytes;
        try
        {
            imageBytes = Convert.FromBase64String(command.LicenseImageBase64);
        }
        catch (FormatException)
        {
            _logger.LogWarning("Invalid base64 for license image for driver with CNPJ {Cnpj}", command.Cnpj);
            throw new DomainException("Imagem da CNH deve ser um base64 válido");
        }

        string contentType = imageBytes.Length > 0 && imageBytes[1] == 0x50 ? "image/png" : "image/bmp";
        if (contentType != "image/png" && contentType != "image/bmp")
        {
            _logger.LogWarning("Invalid file type for license image for driver with CNPJ {Cnpj}", command.Cnpj);
            throw new InvalidFileTypeException("Imagem da CNH deve ser PNG ou BMP");
        }

        string fileName = $"licenses/{driver.Id}_cnh.{(contentType == "image/png" ? "png" : "bmp")}";
        await using var stream = new MemoryStream(imageBytes);
        var imageUrl = await _storageService.UploadFileAsync
            (_bucketName, fileName, stream, contentType, cancellationToken);

        driver.UpdateLicenseImage(imageUrl);

        await _unitOfWork.Drivers.AddAsync(driver, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created driver with ID {DriverId}", driver.Id);

        return driver.Id;
    }
}
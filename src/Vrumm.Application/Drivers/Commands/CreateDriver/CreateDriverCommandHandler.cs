using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions;
using Vrumm.Domain.Exceptions.Drivers;
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
            LicenseTypeValue.Create(command.LicenseType),
            command.LicenseImageBase64);

        await using var stream = new MemoryStream(driver.ImageBytes);
        var imageUrl = await _storageService.UploadFileAsync
            (_bucketName, driver.FileName, stream, driver.ContentType, cancellationToken);

        driver.UpdateLicenseImage(imageUrl);

        await _unitOfWork.Drivers.AddAsync(driver, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created driver with ID {DriverId}", driver.Id);

        return driver.Id;
    }
}
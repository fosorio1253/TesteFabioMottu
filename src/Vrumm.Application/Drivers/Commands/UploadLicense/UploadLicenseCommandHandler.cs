using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Application.Drivers.Commands.UploadLicense;
public class UploadLicenseCommandHandler : ICommandHandler<UploadLicenseCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UploadLicenseCommandHandler> _logger;

    public UploadLicenseCommandHandler(
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        IConfiguration configuration,
        ILogger<UploadLicenseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> Handle(UploadLicenseCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading license image for driver {DriverId}", command.DriverId);

        var driver = await _unitOfWork.Drivers.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver == null)
        {
            _logger.LogWarning("Driver {DriverId} not found", command.DriverId);
            throw new NotFoundException("Driver", command.DriverId);
        }

        try
        {
            var bucketName = _configuration["Storage:BucketName"]
                ?? throw new InvalidOperationException("Storage bucket name not configured.");

            var filePath = $"licenses/{command.DriverId}/{Path.GetFileName(command.FileName)}";
            var uniqueFileName = await _storageService.UploadFileAsync(
                bucketName,
                filePath,
                command.Content,
                command.ContentType,
                cancellationToken);

            driver.UpdateLicenseImage(uniqueFileName);
            await _unitOfWork.Drivers.UpdateAsync(driver);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("License image uploaded for driver {DriverId}: {FilePath}", command.DriverId, uniqueFileName);

            var signedUrl = await _storageService.GetSignedUrlAsync(bucketName, uniqueFileName, cancellationToken, 60);
            return signedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload license image for driver {DriverId}", command.DriverId);
            throw new FileUploadException("Failed to upload license image.", ex);
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Application.Drivers.Commands.UploadLicense;
public class UploadLicenseCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly StorageOptions _options;
    private readonly ILogger<UploadLicenseCommandHandler> _logger;

    public UploadLicenseCommandHandler(
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        IOptions<StorageOptions> options,
        ILogger<UploadLicenseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task<string> Handle(UploadLicenseCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.BucketName))
            throw new InvalidOperationException("Storage bucket name is not configured.");

        var driver = await _unitOfWork.Drivers.GetByIdAsync(command.DriverId, cancellationToken)
            ?? throw new NotFoundException($"Driver {command.DriverId} not found.");

        var filePath = $"licenses/{command.DriverId}/{Path.GetFileName(command.FileName)}";
        var uniqueFileName = await _storageService.UploadFileAsync(
            _options.BucketName,
            filePath,
            command.Content,
            command.ContentType,
            cancellationToken);

        driver.UpdateLicenseImage(uniqueFileName);
        await _unitOfWork.Drivers.UpdateAsync(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var signedUrl = await _storageService.GetSignedUrlAsync(
            _options.BucketName,
            uniqueFileName,
            cancellationToken,
            _options.SignedUrlExpirationMinutes);

        return signedUrl;
    }
}
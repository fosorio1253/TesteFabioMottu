using Microsoft.Extensions.Options;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Options;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Application.Drivers.Commands.UploadLicense;
public class UploadLicenseCommandHandler : ICommandHandler<UploadLicenseCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private int _expirationMinutes;
    private string _bucketName;

    public UploadLicenseCommandHandler(
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        IOptions<GoogleCloudStorageOptions> options)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _bucketName = options.Value.LicenseBucketName;
        _expirationMinutes = options.Value.LicenseSignedUrlExpirationMinutes;
    }

    public async Task<string> Handle
        (UploadLicenseCommand command, CancellationToken cancellationToken)
    {
        var driver = await _unitOfWork.Drivers.GetByIdAsync(command.DriverId, cancellationToken)
            ?? throw new NotFoundException($"Driver {command.DriverId} not found.");

        var filePath = $"licenses/{command.DriverId}/{Path.GetFileName(command.FileName)}";
        var uniqueFileName = await _storageService.UploadFileAsync(
            _bucketName,
            filePath,
            command.Content,
            command.ContentType,
            cancellationToken);

        driver.UpdateLicenseImage(uniqueFileName);
        await _unitOfWork.Drivers.UpdateAsync(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var signedUrl = await _storageService.GetSignedUrlAsync(
            _bucketName,
            uniqueFileName,
            cancellationToken,
            _expirationMinutes);

        return signedUrl;
    }
}
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Infrastructure.Storage.Cloud;
public class CloudStorageService : IStorageService
{
    private readonly CloudStorageOptions _options;
    private readonly ILogger<CloudStorageService> _logger;
    private readonly StorageClient _storageClient;

    public CloudStorageService(IOptions<CloudStorageOptions> options, ILogger<CloudStorageService> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _storageClient = StorageClient.Create();
    }

    public async Task<string> UploadFileAsync(string fileName, Stream content, string contentType)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        if (content == null)
            throw new ArgumentNullException(nameof(content));

        try
        {
            // Generate a unique file name to avoid collisions
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            var uploadOptions = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.Private
            };

            var obj = await _storageClient.UploadObjectAsync(
                bucket: _options.BucketName,
                objectName: uniqueFileName,
                contentType: contentType ?? "application/octet-stream",
                source: content,
                options: uploadOptions);

            _logger.LogInformation("File {FileName} uploaded to GCS bucket {BucketName} as {ObjectName}",
                fileName, _options.BucketName, uniqueFileName);

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to GCS bucket {BucketName}",
                fileName, _options.BucketName);
            throw;
        }
    }

    public async Task<StorageFile> DownloadFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var stream = new MemoryStream();

            var obj = await _storageClient.GetObjectAsync(_options.BucketName, filePath);
            await _storageClient.DownloadObjectAsync(_options.BucketName, filePath, stream);

            stream.Position = 0;

            _logger.LogInformation("File {FilePath} downloaded from GCS bucket {BucketName}",
                filePath, _options.BucketName);

            return new StorageFile(Path.GetFileName(filePath), obj.ContentType, stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {FilePath} from GCS bucket {BucketName}",
                filePath, _options.BucketName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            await _storageClient.DeleteObjectAsync(_options.BucketName, filePath);

            _logger.LogInformation("File {FilePath} deleted from GCS bucket {BucketName}",
                filePath, _options.BucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FilePath} from GCS bucket {BucketName}",
                filePath, _options.BucketName);
            throw;
        }
    }

    public async Task<string> GetSignedUrlAsync(string filePath, int expirationMinutes = 60)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var credential = await GoogleCredential.GetApplicationDefaultAsync();
            if (!(credential is ServiceAccountCredential serviceAccountCredential))
            {
                throw new InvalidOperationException("Credentials are not service account credentials");
            }

            var urlSigner = UrlSigner.FromServiceAccountCredential(serviceAccountCredential);

            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);
            var signedUrl = urlSigner.Sign(_options.BucketName, filePath, expiration);

            _logger.LogInformation("Generated signed URL for file {FilePath} in GCS bucket {BucketName}",
                filePath, _options.BucketName);

            return signedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate signed URL for file {FilePath} in GCS bucket {BucketName}",
                filePath, _options.BucketName);
            throw;
        }
    }
}

public class CloudStorageOptions
{
    public string BucketName { get; set; }
}
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Infrastructure.Storage.Cloud;
public class CloudStorageService : IStorageService
{
    private readonly ILogger<CloudStorageService> _logger;
    private readonly StorageClient _storageClient;

    public CloudStorageService(ILogger<CloudStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storageClient = StorageClient.Create();
    }

    public async Task<string> UploadFileAsync(string bucketName, string fileName, Stream content, string contentType)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        if (content == null)
            throw new ArgumentNullException(nameof(content));

        try
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            var uploadOptions = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.Private
            };

            var obj = await _storageClient.UploadObjectAsync(
                bucket: bucketName,
                objectName: uniqueFileName,
                contentType: contentType ?? "application/octet-stream",
                source: content,
                options: uploadOptions);

            _logger.LogInformation("File {FileName} uploaded to GCS bucket {BucketName} as {ObjectName}",
                fileName, bucketName, uniqueFileName);

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to GCS bucket {BucketName}",
                fileName, bucketName);
            throw;
        }
    }

    public async Task<StorageFile> DownloadFileAsync(string bucketName, string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var stream = new MemoryStream();

            var obj = await _storageClient.GetObjectAsync(bucketName, filePath);
            await _storageClient.DownloadObjectAsync(bucketName, filePath, stream);

            stream.Position = 0;

            _logger.LogInformation("File {FilePath} downloaded from GCS bucket {BucketName}",
                filePath, bucketName);

            return new StorageFile(Path.GetFileName(filePath), obj.ContentType, stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {FilePath} from GCS bucket {BucketName}",
                filePath, bucketName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string bucketName, string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            await _storageClient.DeleteObjectAsync(bucketName, filePath);

            _logger.LogInformation("File {FilePath} deleted from GCS bucket {BucketName}",
                filePath, bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FilePath} from GCS bucket {BucketName}",
                filePath, bucketName);
            throw;
        }
    }

    public async Task<string> GetSignedUrlAsync(string bucketName, string filePath, int expirationMinutes = 60)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var credential = await GoogleCredential.GetApplicationDefaultAsync();
            if (credential.UnderlyingCredential is not ServiceAccountCredential serviceAccountCredential)
                throw new InvalidOperationException("Credentials are not service account credentials");

            var urlSigner = UrlSigner.FromCredential(serviceAccountCredential);

            var expiration = TimeSpan.FromMinutes(expirationMinutes);
            var signedUrl = urlSigner.Sign(bucketName, filePath, expiration);

            _logger.LogInformation("Generated signed URL for file {FilePath} in GCS bucket {BucketName}",
                filePath, bucketName);

            return signedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate signed URL for file {FilePath} in GCS bucket {BucketName}",
                filePath, bucketName);
            throw;
        }
    }
}
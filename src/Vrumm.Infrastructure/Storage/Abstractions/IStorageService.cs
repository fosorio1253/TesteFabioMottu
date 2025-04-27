namespace Vrumm.Infrastructure.Storage.Abstractions;
public interface IStorageService
{
    Task<string> UploadFileAsync(string bucketName, string fileName, Stream content, string contentType, CancellationToken cancellationToken);
    Task<StorageFile> DownloadFileAsync(string bucketName, string filePath, CancellationToken cancellationToken);
    Task DeleteFileAsync(string bucketName, string filePath, CancellationToken cancellationToken);
    Task<string> GetSignedUrlAsync(string bucketName, string filePath, CancellationToken cancellationToken, int expirationMinutes = 60);
}
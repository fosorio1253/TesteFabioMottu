namespace Vrumm.Infrastructure.Storage.Abstractions;
public interface IStorageService
{
    Task<string> UploadFileAsync(string bucketName, string fileName, Stream content, string contentType);
    Task<StorageFile> DownloadFileAsync(string bucketName, string filePath);
    Task DeleteFileAsync(string bucketName, string filePath);
    Task<string> GetSignedUrlAsync(string bucketName, string filePath, int expirationMinutes = 60);
}
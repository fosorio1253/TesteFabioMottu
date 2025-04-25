namespace Vrumm.Infrastructure.Storage.Abstractions;
public interface IStorageService
{
    Task<string> UploadFileAsync(string fileName, Stream content, string contentType);
    Task<StorageFile> DownloadFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    Task<string> GetSignedUrlAsync(string filePath, int expirationMinutes = 60);
}
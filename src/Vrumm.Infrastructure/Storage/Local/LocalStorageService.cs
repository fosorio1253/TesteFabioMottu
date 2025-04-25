using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Infrastructure.Storage.Local;
public class LocalStorageService : IStorageService
{
    private readonly LocalStorageOptions _options;
    private readonly ILogger<LocalStorageService> _logger;
    private readonly IWebHostEnvironment _environment;

    public LocalStorageService(
        IOptions<LocalStorageOptions> options,
        ILogger<LocalStorageService> logger,
        IWebHostEnvironment environment)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));

        EnsureStorageDirectoryExists();
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
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var filePath = Path.Combine(GetStoragePath(), uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await content.CopyToAsync(fileStream);
            }

            _logger.LogInformation("File {FileName} uploaded to {FilePath}", fileName, filePath);

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName}", fileName);
            throw;
        }
    }

    public async Task<StorageFile> DownloadFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var fullPath = Path.Combine(GetStoragePath(), filePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File {FilePath} not found", fullPath);
                return null;
            }

            var content = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(content);
            }

            content.Position = 0;

            var fileName = Path.GetFileName(filePath);
            var contentType = GetContentTypeFromFileName(fileName);

            _logger.LogInformation("File {FilePath} downloaded", fullPath);

            return new StorageFile(fileName, contentType, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {FilePath}", filePath);
            throw;
        }
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var fullPath = Path.Combine(GetStoragePath(), filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File {FilePath} deleted", fullPath);
            }
            else
            {
                _logger.LogWarning("File {FilePath} not found for deletion", fullPath);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FilePath}", filePath);
            throw;
        }
    }

    public Task<string> GetSignedUrlAsync(string filePath, int expirationMinutes = 60)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            // In local environment, we just return a relative URL to the file
            var fileName = Path.GetFileName(filePath);
            var url = $"/api/files/{fileName}";

            _logger.LogInformation("Generated URL {Url} for file {FilePath}", url, filePath);

            return Task.FromResult(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate URL for file {FilePath}", filePath);
            throw;
        }
    }

    private string GetStoragePath()
    {
        return string.IsNullOrEmpty(_options.Path)
            ? Path.Combine(_environment.ContentRootPath, "storage")
            : _options.Path;
    }

    private void EnsureStorageDirectoryExists()
    {
        var storagePath = GetStoragePath();

        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
            _logger.LogInformation("Created storage directory at {StoragePath}", storagePath);
        }
    }

    private string GetContentTypeFromFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}

public class LocalStorageOptions
{
    public string Path { get; set; }
}
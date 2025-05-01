using FluentAssertions;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Infrastructure.Storage.GoogleCloud;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Storage;
public class GoogleCloudStorageServiceTests
{
    private readonly Mock<ILogger<GoogleCloudStorageService>> _loggerMock;
    private readonly Mock<StorageClient> _storageClientMock;
    private readonly GoogleCloudStorageService _service;

    public GoogleCloudStorageServiceTests()
    {
        _loggerMock = new Mock<ILogger<GoogleCloudStorageService>>();
        _storageClientMock = new Mock<StorageClient>();
        _service = new GoogleCloudStorageService(_loggerMock.Object);

        var fieldInfo = typeof(GoogleCloudStorageService).GetField(
            "_storageClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        fieldInfo?.SetValue(_service, _storageClientMock.Object);
    }
    /*
    [Fact]
    public async Task UploadFileAsync_ValidInput_ReturnsUniqueFileName()
    {
        // Arrange
        var bucketName = "test-bucket";
        var fileName = "test.png";
        var content = new MemoryStream(new byte[] { 0x89, 0x50, 0x4E, 0x47 });
        var contentType = "image/png";
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var uploadedObject = new Google.Apis.Storage.v1.Data.Object { Name = uniqueFileName };

        _storageClientMock.Setup(c => c.UploadObjectAsync(
            bucketName,
            uniqueFileName,
            contentType,
            content,
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadedObject);

        // Act
        var result = await _service.UploadFileAsync(bucketName, fileName, content, contentType, CancellationToken.None);

        // Assert
        result.Should().Be(uniqueFileName);
        _loggerMock.VerifyLog(LogLevel.Information, $"File {fileName} uploaded to GCS bucket {bucketName} as {uniqueFileName}");
    }

    [Fact]
    public async Task DownloadFileAsync_ValidFilePath_ReturnsStorageFile()
    {
        // Arrange
        var bucketName = "test-bucket";
        var filePath = "test.png";
        var contentType = "image/png";
        var content = new MemoryStream(new byte[] { 0x89, 0x50, 0x4E, 0x47 });
        var obj = new Google.Apis.Storage.v1.Data.Object { ContentType = contentType };

        _storageClientMock.Setup(c => c.GetObjectAsync
        (bucketName, filePath, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(obj);

        _storageClientMock.Setup(c => c.DownloadObjectAsync(
            bucketName,
            filePath,
            It.IsAny<Stream>(),
            null,
            It.IsAny<CancellationToken>()))
            .Callback((
                string _,
                string _,
                Stream stream, DownloadObjectOptions _,
                CancellationToken _)
                => content.CopyTo(stream));

        // Act
        var result = await _service.DownloadFileAsync(bucketName, filePath, CancellationToken.None);

        // Assert
        result.FileName.Should().Be("test.png");
        result.ContentType.Should().Be(contentType);
        result.Content.Length.Should().Be(content.Length);
    }

    [Fact]
    public async Task DeleteFileAsync_ValidFilePath_ExecutesWithoutError()
    {
        // Arrange
        var bucketName = "test-bucket";
        var filePath = "test.png";

        _storageClientMock.Setup(c => c.DeleteObjectAsync
        (bucketName, filePath, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteFileAsync(bucketName, filePath, CancellationToken.None);

        // Assert
        _loggerMock.VerifyLog(LogLevel.Information, $"File {filePath} deleted from GCS bucket {bucketName}");
    }
}

// Extensão para verificar logs
public static class LoggerMockExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, string message)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }
    */
}
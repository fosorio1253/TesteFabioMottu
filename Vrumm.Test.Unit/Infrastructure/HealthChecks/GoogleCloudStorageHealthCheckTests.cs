using FluentAssertions;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Vrumm.Infrastructure.Dependency.Configurations;
using Vrumm.Infrastructure.Storage.GoogleCloud;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.HealthChecks;
public class GoogleCloudStorageHealthCheckTests
{
    private readonly Mock<ILogger<GoogleCloudStorageHealthCheck>> _loggerMock;
    private readonly Mock<IOptions<GoogleCloudOptions>> _optionsMock;
    private readonly Mock<StorageClient> _storageClientMock;
    private readonly GoogleCloudStorageHealthCheck _healthCheck;

    public GoogleCloudStorageHealthCheckTests()
    {
        _loggerMock = new Mock<ILogger<GoogleCloudStorageHealthCheck>>();
        _optionsMock = new Mock<IOptions<GoogleCloudOptions>>();
        _storageClientMock = new Mock<StorageClient>();
        _healthCheck = new GoogleCloudStorageHealthCheck(_optionsMock.Object, _loggerMock.Object);

        var fieldInfo = typeof(GoogleCloudStorageHealthCheck).GetField("_storageClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fieldInfo?.SetValue(_healthCheck, _storageClientMock.Object);
    }
    /*
    [Fact]
    public async Task CheckHealthAsync_WhenStorageIsHealthy_ReturnsHealthy()
    {
        // Arrange
        _storageClientMock.Setup(c => c.ListBucketsAsync
        (It.IsAny<string>(), It.IsAny<ListBucketsOptions>()))
            .ReturnsAsync(new List<Bucket>().AsEnumerable());

        // Act
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenStorageFails_ReturnsUnhealthy()
    {
        // Arrange
        _storageClientMock.Setup(c => c.ListBucketsAsync(
            It.IsAny<string>(),
            It.IsAny<ListBucketsOptions>()))
            .ThrowsAsync(new Exception("Storage error"));

        // Act
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Exception.Message.Should().Be("Storage error");
    }
    */
}
using FluentAssertions;
using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Vrumm.Infrastructure.Dependency.Configurations;
using Vrumm.Infrastructure.Messaging.PubSub;
using Vrumm.Infrastructure.Storage.GoogleCloud;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.HealthChecks;
public class PubSubHealthCheckTests
{
    private readonly Mock<ILogger<PubSubHealthCheck>> _loggerMock;
    private readonly Mock<IOptions<GoogleCloudOptions>> _optionsMock;
    private readonly Mock<PublisherServiceApiClient> _publisherClientMock;
    private readonly PubSubHealthCheck _healthCheck;

    public PubSubHealthCheckTests()
    {
        _loggerMock = new Mock<ILogger<PubSubHealthCheck>>();
        _optionsMock = new Mock<IOptions<GoogleCloudOptions>>();
        _publisherClientMock = new Mock<PublisherServiceApiClient>();
        _healthCheck = new PubSubHealthCheck(_optionsMock.Object, _loggerMock.Object);

        var fieldInfo = typeof(PubSubHealthCheck).GetField(
            "_publisherClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        fieldInfo?.SetValue(_healthCheck, _publisherClientMock.Object);
    }
    /*
    [Fact]
    public async Task CheckHealthAsync_WhenPubSubIsHealthy_ReturnsHealthy()
    {
        // Arrange
        _publisherClientMock.Setup(c => c.ListTopicsAsync(
            It.IsAny<ListTopicsRequest>(),
            It.IsAny<CallSettings>()))
            .ReturnsAsync(new ListTopicsResponse());

        // Act
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenPubSubFails_ReturnsUnhealthy()
    {
        // Arrange
        _publisherClientMock.Setup(c => c.ListTopicsAsync(It.IsAny<ListTopicsRequest>(), It.IsAny<CallOptions>()))
            .ThrowsAsync(new Exception("PubSub error"));

        // Act
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Exception.Message.Should().Be("PubSub error");
    }
    */
}

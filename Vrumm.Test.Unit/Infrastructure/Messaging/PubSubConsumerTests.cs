using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Vrumm.Infrastructure.Dependency.Configurations;
using Vrumm.Infrastructure.Messaging.PubSub;
using Xunit;
using static Google.Cloud.PubSub.V1.Subscriber;

namespace Vrumm.Test.Unit.Infrastructure.Messaging;
public class PubSubConsumerTests
{
    private readonly Mock<IOptions<GoogleCloudOptions>> _optionsMock;
    private readonly Mock<ILogger<PubSubConsumer>> _loggerMock;
    private readonly Mock<SubscriberClient> _subscriberClientMock;
    private readonly PubSubConsumer _consumer;

    public PubSubConsumerTests()
    {
        _optionsMock = new Mock<IOptions<GoogleCloudOptions>>();
        _loggerMock = new Mock<ILogger<PubSubConsumer>>();
        _subscriberClientMock = new Mock<SubscriberClient>();
        _consumer = new PubSubConsumer(_optionsMock.Object, _loggerMock.Object);

        var fieldInfo = typeof(PubSubConsumer).GetField(
            "_subscriberClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        fieldInfo?.SetValue(_consumer, _subscriberClientMock.Object);
    }
    /*
    [Fact]
    public async Task StartAsync_ValidSubscription_StartsSubscriber()
    {
        // Arrange
        var subscriptionName = "test-subscription";
        _subscriberClientMock.Setup(c => c.StartAsync(It.IsAny<Func<PubsubMessage, CancellationToken, Task<SubscriberClient.Reply>>>()))
            .ReturnsAsync(SubscriberClient.SubscriberState.Stopped);

        // Act
        await _consumer.StartAsync(CancellationToken.None);

        // Assert
        _subscriberClientMock.Verify(c => c.StartAsync(It.IsAny<Func<PubsubMessage, CancellationToken, Task<SubscriberClient.Reply>>>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Started consuming messages from subscription {subscriptionName}");
    }

    [Fact]
    public async Task StopAsync_WhenRunning_StopsSubscriber()
    {
        // Arrange
        _subscriberClientMock.Setup(c => c.StopAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.StopAsync(CancellationToken.None);

        // Assert
        _subscriberClientMock.Verify(c => c.StopAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Stopped consuming messages");
    }
    */
}
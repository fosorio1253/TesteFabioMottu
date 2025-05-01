using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Vrumm.Domain.Events;
using Vrumm.Infrastructure.Dependency.Configurations;
using Vrumm.Infrastructure.Messaging.PubSub;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Messaging;
public class PubSubPublisherTests
{
    private readonly Mock<ILogger<PubSubPublisher>> _loggerMock;
    private readonly Mock<IOptions<GoogleCloudOptions>> _optionsMock;
    private readonly Mock<PublisherClient> _publisherClientMock;
    private readonly PubSubPublisher _publisher;

    public PubSubPublisherTests()
    {
        _loggerMock = new Mock<ILogger<PubSubPublisher>>();
        _publisherClientMock = new Mock<PublisherClient>();
        _optionsMock = new Mock<IOptions<GoogleCloudOptions>>();
        _publisher = new PubSubPublisher(_optionsMock.Object, _loggerMock.Object);
        
        var fieldInfo = typeof(PubSubPublisher).GetField(
            "_publisherClient",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        fieldInfo?.SetValue(_publisher, _publisherClientMock.Object);
    }
    /*
    [Fact]
    public async Task PublishAsync_ValidMessage_PublishesSuccessfully()
    {
        // Arrange
        var topicName = "test-topic";
        var rentalFinalized = new RentalFinalized(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            10.10m);

        _publisherClientMock.Setup(c => c.PublishAsync(It.IsAny<PubsubMessage>()))
            .ReturnsAsync("message-id");

        // Act
        await _publisher.PublishAsync(rentalFinalized, topicName);

        // Assert
        _publisherClientMock.Verify(c => c.PublishAsync(It.Is<PubsubMessage>(m => m.Data.ToStringUtf8() == rentalFinalized.ToString())), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Published message to topic {topicName}");
    }

    [Fact]
    public async Task PublishAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var rentalFinalized = new RentalFinalized(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            10.10m);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => _publisher.PublishAsync(rentalFinalized, null));
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
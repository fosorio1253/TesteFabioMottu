]using System.Text.Json;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Domain.Commom;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Infrastructure.Messaging.PubSub;
public class PubSubPublisher : IMessagePublisher
{
    private readonly PubSubOptions _options;
    private readonly ILogger<PubSubPublisher> _logger;

    public PubSubPublisher(IOptions<PubSubOptions> options, ILogger<PubSubPublisher> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishAsync<T>(T message, string topic = null) where T : DomainEvent
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            var topicName = topic ?? message.GetType().Name;
            var fullTopicName = new TopicName(_options.ProjectId, topicName);

            PublisherClient publisher = await PublisherClient.CreateAsync(fullTopicName);

            var envelope = new MessageEnvelope<T>(message);
            var messageJson = JsonSerializer.Serialize(envelope);
            var pubsubMessage = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(messageJson),
                // Add attributes for filtering if needed
                Attributes =
                    {
                        { "MessageType", typeof(T).Name },
                        { "MessageId", message.Id.ToString() }
                    }
            };

            string messageId = await publisher.PublishAsync(pubsubMessage);

            _logger.LogInformation("Published message {MessageType} with ID {MessageId}", typeof(T).Name, messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }
}

public class PubSubOptions
{
    public string ProjectId { get; set; }
}
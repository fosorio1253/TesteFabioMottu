using System.Text.Json.Serialization;

namespace Vrumm.Infrastructure.Messaging.Abstractions;
public class MessageEnvelope<T>
{
    public Guid MessageId { get; set; }
    public DateTime Timestamp { get; set; }
    public string MessageType { get; set; }
    public T Payload { get; set; }

    [JsonConstructor]
    public MessageEnvelope(Guid messageId, DateTime timestamp, string messageType, T payload)
    {
        MessageId = messageId;
        Timestamp = timestamp;
        MessageType = messageType;
        Payload = payload;
    }

    public MessageEnvelope(T payload)
    {
        MessageId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        MessageType = typeof(T).Name;
        Payload = payload;
    }
}
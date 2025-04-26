using Vrumm.Domain.Common;

namespace Vrumm.Infrastructure.Messaging.Abstractions;
public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string topic = null) where T : DomainEvent;
}
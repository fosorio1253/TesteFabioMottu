namespace Vrumm.Infrastructure.Messaging.Abstractions;
public interface IMessageConsumer
{
    Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class;
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}
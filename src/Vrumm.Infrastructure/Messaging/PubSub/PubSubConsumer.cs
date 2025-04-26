using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Vrumm.Infrastructure.Dependency.Configurations;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Infrastructure.Messaging.PubSub;
public class PubSubConsumer : IMessageConsumer, IHostedService
{
    private readonly GoogleCloudOptions _options;
    private readonly ILogger<PubSubConsumer> _logger;
    private readonly string _subscriptionId;
    private SubscriberClient _subscriber;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly Dictionary<string, Delegate> _handlers = new Dictionary<string, Delegate>();

    public PubSubConsumer(IOptions<GoogleCloudOptions> options, ILogger<PubSubConsumer> logger, string subscriptionId)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (string.IsNullOrEmpty(subscriptionId))
            throw new ArgumentException("Subscription ID cannot be null or empty", nameof(subscriptionId));
        _subscriptionId = subscriptionId;
    }

    public Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class
    {
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        _logger.LogInformation("Subscribed to topic {Topic} with subscription {SubscriptionId}", topic, _subscriptionId);
        _handlers[topic] = handler;
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var subscriptionName = new SubscriptionName(_options.ProjectId, _subscriptionId);
        _subscriber = await SubscriberClient.CreateAsync(subscriptionName);

        _logger.LogInformation("Starting subscriber for subscription {SubscriptionId}", _subscriptionId);
        await _subscriber.StartAsync((message, token) =>
        {
            try
            {
                var data = message.Data.ToStringUtf8();
                _logger.LogInformation("Received message: {Data}", data);

                string topic = "";
                if (message.Attributes.TryGetValue("topic", out var messageTopic))
                {
                    topic = messageTopic;
                }

                if (!string.IsNullOrEmpty(topic) && _handlers.TryGetValue(topic, out var handler))
                {
                    var messageType = handler.GetType().GetGenericArguments()[0];
                    var messageObject = JsonConvert.DeserializeObject(data, messageType);

                    var method = handler.GetType().GetMethod("Invoke");
                    if (method != null)
                    {
                        Task handlerTask = (Task)method.Invoke(handler, new[] { messageObject });
                        handlerTask.Wait(token);
                    }
                }

                return Task.FromResult(SubscriberClient.Reply.Ack);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                return Task.FromResult(SubscriberClient.Reply.Nack);
            }
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping subscriber for subscription {SubscriptionId}", _subscriptionId);
        _cancellationTokenSource?.Cancel();

        if (_subscriber != null)
        {
            await _subscriber.StopAsync(cancellationToken);
            _subscriber = null;
        }

        _logger.LogInformation("Subscriber stopped for subscription {SubscriptionId}", _subscriptionId);
    }
}
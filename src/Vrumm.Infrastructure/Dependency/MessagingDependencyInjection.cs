using Microsoft.Extensions.DependencyInjection;
using Vrumm.Infrastructure.Messaging.Abstractions;
using Vrumm.Infrastructure.Messaging.PubSub;

namespace Vrumm.Infrastructure.Dependency;
public static class MessagingDependencyInjection
{
    public static IServiceCollection AddMessagingDependecies
        (this IServiceCollection services, IHealthChecksBuilder hcBuilder,
        Action<MessagingOptions> configureOptions = null)
    {
        var messagingOptions = new MessagingOptions();
        configureOptions?.Invoke(messagingOptions);

        services.AddSingleton<IMessagePublisher, PubSubPublisher>();

        hcBuilder.AddCheck<PubSubHealthCheck>(
            messagingOptions.GoogleCloudMessaging.Name,
            tags: messagingOptions.GoogleCloudMessaging.Tags);

        return services;
    }

    public class MessagingOptions
    {
        public GoogleCloudMessagingOptions GoogleCloudMessaging { get; set; } = new GoogleCloudMessagingOptions();

        public class GoogleCloudMessagingOptions
        {
            public string Name { get; set; }
            public List<string> Tags { get; set; }
        }
    }
}
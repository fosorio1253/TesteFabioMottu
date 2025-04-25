using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Logging;
using Vrumm.Infrastructure.Messaging.Abstractions;
using Vrumm.Infrastructure.Messaging.PubSub;
using Vrumm.Infrastructure.Messaging.RabbitMQ;
using Vrumm.Infrastructure.Storage.Abstractions;
using Vrumm.Infrastructure.Storage.Cloud;
using Vrumm.Infrastructure.Storage.Local;

namespace Vrumm.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<VrummDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(VrummDbContext).Assembly.FullName));
        });

        // Repositories
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // MediatR pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Storage
        ConfigureStorage(services, configuration);

        // Messaging
        ConfigureMessaging(services, configuration);

        return services;
    }

    private static void ConfigureStorage(IServiceCollection services, IConfiguration configuration)
    {
        var storageType = configuration["Storage:Type"];

        switch (storageType?.ToLowerInvariant())
        {
            case "gcs":
                services.Configure<CloudStorageOptions>(configuration.GetSection("Storage:GCS"));
                services.AddSingleton<IStorageService, CloudStorageService>();
                break;

            case "local":
            default:
                services.Configure<LocalStorageOptions>(configuration.GetSection("Storage:Local"));
                services.AddSingleton<IStorageService, LocalStorageService>();
                break;
        }
    }

    private static void ConfigureMessaging(IServiceCollection services, IConfiguration configuration)
    {
        var messagingType = configuration["Messaging:Type"];

        switch (messagingType?.ToLowerInvariant())
        {
            case "pubsub":
                services.Configure<PubSubOptions>(configuration.GetSection("Messaging:PubSub"));
                services.AddSingleton<IMessagePublisher, PubSubPublisher>();

                // Register consumers as hosted services if needed
                // Example: services.AddHostedService<YourConsumerService>();
                break;

            case "rabbitmq":
            default:
                services.Configure<RabbitMQOptions>(configuration.GetSection("Messaging:RabbitMQ"));
                services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

                // Add named consumers if needed
                services.AddSingleton<IMessageConsumer>(provider =>
                    new RabbitMQConsumer(
                        provider.GetRequiredService<IOptions<RabbitMQOptions>>(),
                        provider.GetRequiredService<ILogger<RabbitMQConsumer>>(),
                        "motorcycle-events"));
                break;
        }
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        // Database health check
        hcBuilder.AddNpgSql(
            configuration.GetConnectionString("PostgreSQL"),
            name: "postgresql",
            tags: new[] { "db", "data" });

        // Storage health check
        var storageType = configuration["Storage:Type"];
        if (storageType?.ToLowerInvariant() == "gcs")
        {
            hcBuilder.AddCheck<GcsHealthCheck>(
                "gcs-storage",
                tags: new[] { "storage" });
        }

        // Messaging health check
        var messagingType = configuration["Messaging:Type"];
        if (messagingType?.ToLowerInvariant() == "rabbitmq")
        {
            var rabbitOptions = new RabbitMQOptions();
            configuration.GetSection("Messaging:RabbitMQ").Bind(rabbitOptions);

            hcBuilder.AddRabbitMQ(
                $"amqp://{rabbitOptions.Username}:{rabbitOptions.Password}@{rabbitOptions.Host}:{rabbitOptions.Port}/{rabbitOptions.VirtualHost}",
                name: "rabbitmq",
                tags: new[] { "messaging" });
        }
        else if (messagingType?.ToLowerInvariant() == "pubsub")
        {
            hcBuilder.AddCheck<PubSubHealthCheck>(
                "pubsub",
                tags: new[] { "messaging" });
        }

        return services;
    }
}
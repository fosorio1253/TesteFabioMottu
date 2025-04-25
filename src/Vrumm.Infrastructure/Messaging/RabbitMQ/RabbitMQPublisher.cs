using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Vrumm.Domain.Commom;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Infrastructure.Messaging.RabbitMQ;
public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private bool _disposed;

    public RabbitMQPublisher(IOptions<RabbitMQOptions> options, ILogger<RabbitMQPublisher> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            UserName = _options.Username,
            Password = _options.Password,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }

    public Task PublishAsync<T>(T message, string topic = null) where T : DomainEvent
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            var routingKey = topic ?? message.GetType().Name;
            var envelope = new MessageEnvelope<T>(message);
            var messageJson = JsonSerializer.Serialize(envelope);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = message.Id.ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: messageBytes);

            _logger.LogInformation("Published message of type {MessageType} with ID {MessageId}", typeof(T).Name, message.Id);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        _disposed = true;
    }
}

public class RabbitMQOptions
{
    public string Host { get; set; } = "localhost";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "vrumm-events";
}
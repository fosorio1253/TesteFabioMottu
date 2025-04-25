using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Infrastructure.Messaging.RabbitMQ;
public class RabbitMQConsumer : IMessageConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private readonly string _queueName;
    private bool _disposed;

    public RabbitMQConsumer(IOptions<RabbitMQOptions> options, ILogger<RabbitMQConsumer> logger, string consumerName)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrEmpty(consumerName))
            throw new ArgumentException("Consumer name cannot be null or empty", nameof(consumerName));

        _queueName = $"vrumm-{consumerName}";

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

            // Declare queue
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }

    public Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class
    {
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        _channel.QueueBind(
            queue: _queueName,
            exchange: _options.ExchangeName,
            routingKey: topic);

        _logger.LogInformation("Subscribed to topic {Topic} on queue {QueueName}", topic, _queueName);

        return Task.CompletedTask;
    }

    public Task StartAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = args.RoutingKey;

                _logger.LogInformation("Received message with routing key {RoutingKey}", routingKey);

                // Process message based on routing key
                // Here we would handle different types of messages
                // For now, we just acknowledge the message

                _channel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _channel.BasicNack(args.DeliveryTag, false, true);
            }

            await Task.CompletedTask;
        };

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Started consuming messages from queue {QueueName}", _queueName);

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _logger.LogInformation("Stopped consuming messages from queue {QueueName}", _queueName);
        return Task.CompletedTask;
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
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;

namespace ThreatIntelligencePlatform.MessageBroker.Services;

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private readonly RabbitMQOptions _rabbitOptions;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IConnection _connection;
    private readonly ConcurrentDictionary<string, IModel> _channels;
    private readonly ILogger<RabbitMQService> _logger;
    private bool _disposed;
    
    public RabbitMQService(IOptions<RabbitMQOptions> rabbitOptions, ILogger<RabbitMQService> logger)
    {
        _rabbitOptions = rabbitOptions.Value;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = false };
        _logger = logger;
        try
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitOptions.HostName,
                Port = _rabbitOptions.Port,
                UserName = _rabbitOptions.UserName,
                Password = _rabbitOptions.Password,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            _connection = _connectionFactory.CreateConnection();
            _channels = new ConcurrentDictionary<string, IModel>();
            _logger.LogInformation("Successfully connected to RabbitMQ");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }
    
    private IModel GetOrAddChannel(string purpose)
    {
        return _channels.GetOrAdd(purpose, _ =>
        {
            var channel = _connection.CreateModel();
            channel.ConfirmSelect();
            return channel;
        });
    }

    public void DeclareExchange(string exchangeName, string exchangeType, bool durable = true)
    {
        ThrowIfDisposed();
        var channel = GetOrAddChannel("declare");
        channel.ExchangeDeclare(exchangeName, exchangeType, durable);
        _logger.LogInformation("Declared exchange: {ExchangeName}, Type: {ExchangeType}, Durable: {Durable}", 
            exchangeName, exchangeType, durable);
    }

    public void DeclareQueue(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
    {
        ThrowIfDisposed();
        var channel = GetOrAddChannel("declare");
        channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
        _logger.LogInformation(
            "Declared queue: {QueueName}, Durable: {Durable}, Exclusive: {Exclusive}, AutoDelete: {AutoDelete}", 
            queueName, durable, exclusive, autoDelete);
    }

    public void BindQueue(string queueName, string exchangeName, string routingKey)
    {
        ThrowIfDisposed();
        var channel = GetOrAddChannel("bind");
        channel.QueueBind(queueName, exchangeName, routingKey);
        _logger.LogInformation("Bound queue {QueueName} to exchange {ExchangeName} with routing key {RoutingKey}", 
            queueName, exchangeName, routingKey);
    }

    public void Publish<T>(string exchangeName, string routingKey, T message) where T : class
    {
        ThrowIfDisposed();
        try
        {
            var channel = GetOrAddChannel("publish");
            var body = JsonSerializer.SerializeToUtf8Bytes(message, _jsonOptions);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
                
            channel.BasicPublish(exchangeName, routingKey, properties, body);
            _logger.LogInformation(
                "Published message of type {MessageType} to exchange {ExchangeName} with routing key {RoutingKey}", 
                typeof(T).Name, exchangeName, routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public void Subscribe<T>(string queueName, Func<T, Task> messageHandler, ushort prefetchCount = 1) where T : class
    {
        ThrowIfDisposed();
        try
        {
            var channel = GetOrAddChannel($"subscribe_{queueName}");
            channel.BasicQos(0, prefetchCount, false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<T>(body, _jsonOptions);
                    await messageHandler(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Processed message of type {MessageType} from queue {QueueName}", 
                        typeof(T).Name, queueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message of type {MessageType} from queue {QueueName}", 
                        typeof(T).Name, queueName);
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(queueName, false, consumer);
            _logger.LogInformation("Subscribed to queue {QueueName} for messages of type {MessageType}", 
                queueName, typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set up consumer for queue {QueueName}", queueName);
            throw;
        } 
    }
    
    public void Unsubscribe(string queueName)
    {
        ThrowIfDisposed();
        if (_channels.TryRemove($"subscribe_{queueName}", out var channel))
        {
            channel.Close();
            channel.Dispose();
            _logger.LogInformation("Unsubscribed from queue {QueueName}", queueName);
        }
    }
    
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMQService));
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
            
        foreach (var channel in _channels.Values)
        {
            channel.Close();
            channel.Dispose();
        }
        _channels.Clear();
        
        _connection.Close();
        _connection.Dispose();
            
        _logger.LogInformation("RabbitMQ connections and channels disposed");
        _disposed = true;
    }
}
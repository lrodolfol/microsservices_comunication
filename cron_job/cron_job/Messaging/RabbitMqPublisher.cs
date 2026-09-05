using System.Text;
using System.Text.Json;
using cron_job.Settings;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace cron_job.Messaging;

public sealed class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueName;
    private readonly ILogger<RabbitMqPublisher> _logger;

    private static readonly string[] TypeRequests = ["Rest", "Grpc"];
    private static readonly Random _random = new();

    private RabbitMqPublisher(IConnection connection, IChannel channel, string queueName, ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _channel = channel;
        _queueName = queueName;
        _logger = logger;
    }

    public static async Task<RabbitMqPublisher> CreateAsync(RabbitMqSettings settings, ILogger<RabbitMqPublisher> logger)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.User,
            Password = settings.Password
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        return new RabbitMqPublisher(connection, channel, settings.QueueName, logger);
    }

    public async Task PublishAsync()
    {
        var correlationId = Guid.NewGuid().ToString();
        var typeRequest = "Rest"; //TypeRequests[_random.Next(TypeRequests.Length)];

        var payload = new MessagePayload { CorrelationId = correlationId, TypeRequest = typeRequest };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: _queueName, body: body);

        Counter.Increment();
        _logger.LogInformation("Message published. CorrelationId={CorrelationId}, TypeRequest={TypeRequest}. Counter {counter}", correlationId, typeRequest, Counter.Value);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}

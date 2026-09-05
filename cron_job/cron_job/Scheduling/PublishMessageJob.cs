using cron_job.Messaging;
using Microsoft.Extensions.Logging;

namespace cron_job.Scheduling;

public class PublishMessageJob
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<PublishMessageJob> _logger;

    public PublishMessageJob(IMessagePublisher publisher, ILogger<PublishMessageJob> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public void Execute()
    {
        try
        {
            _publisher.PublishAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to RabbitMQ");
        }
    }
}

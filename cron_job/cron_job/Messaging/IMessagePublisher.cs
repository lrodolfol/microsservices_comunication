namespace cron_job.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync();
}

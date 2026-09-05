namespace cron_job.Settings;

public class RabbitMqSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
}

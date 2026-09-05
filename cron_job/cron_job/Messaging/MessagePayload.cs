namespace cron_job.Messaging;

public class MessagePayload
{
    public string CorrelationId { get; set; } = string.Empty;
    public string TypeRequest { get; set; } = string.Empty;
}

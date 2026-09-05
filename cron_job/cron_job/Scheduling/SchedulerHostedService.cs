using cron_job.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace cron_job.Scheduling;

public class SchedulerHostedService : BackgroundService
{
    private readonly PublishMessageJob _job;
    private readonly double _intervalSeconds;

    public SchedulerHostedService(PublishMessageJob job, IOptions<SchedulerSettings> settings)
    {
        _job = job;
        _intervalSeconds = settings.Value.IntervalSeconds;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _job.Execute();
        
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalSeconds));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            Parallel.For(0, 2, _ =>
            {
                _job.Execute();
            });
        }
    }
}

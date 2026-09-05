using cron_job.Messaging;
using cron_job.Scheduling;
using cron_job.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
       ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
       ?? "Development";

builder.Configuration.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

var resource = ResourceBuilder.CreateDefault().AddService("cron_job", serviceVersion: "1.0");

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
    options.SingleLine = true;
    options.UseUtcTimestamp = false;
});
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resource);
    options.AddOtlpExporter(otlp =>
    {
        otlp.Endpoint = new Uri("http://localhost:4317");
    });
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("cron_job"))
    .WithMetrics(metrics =>
    {
        metrics.AddRuntimeInstrumentation();
        metrics.AddHttpClientInstrumentation();

        metrics.AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri("http://localhost:4317");
        });
    })
    .WithTracing(tracing =>
    {
        tracing.AddHttpClientInstrumentation();

        tracing.AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri("http://localhost:4317");
        });
    });

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<SchedulerSettings>(builder.Configuration.GetSection("Scheduler"));

builder.Services.AddSingleton<IMessagePublisher>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
    var logger = sp.GetRequiredService<ILogger<RabbitMqPublisher>>();
    return RabbitMqPublisher.CreateAsync(settings, logger).GetAwaiter().GetResult();
});

builder.Services.AddSingleton<PublishMessageJob>();
builder.Services.AddHostedService<SchedulerHostedService>();

var host = builder.Build();
await host.RunAsync();

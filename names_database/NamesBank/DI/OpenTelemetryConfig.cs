namespace NamesBank.DI;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;

public static class OpenTelemetryConfig
{
    public static void AddOtelConfig(this WebApplicationBuilder builder, string appName)
    {
        var resource = ResourceBuilder.CreateDefault().AddService(appName, serviceVersion: "1.0");
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
                otlp.Endpoint = new Uri("http://localhost:4317"); // Collector
            });
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(appName))
            .WithMetrics(metrics =>
            {
                metrics.AddRuntimeInstrumentation();
                metrics.AddProcessInstrumentation();
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();

                metrics.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri("http://localhost:4317");
                });
            })
            // ===== TRACES =====
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();

                tracing.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri("http://localhost:4317");
                });
            });
    }
}
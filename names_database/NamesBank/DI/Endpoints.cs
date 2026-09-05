using NamesBank.Gateways;
using NamesBank.Models;
using System.Diagnostics;

namespace NamesBank.DI;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app, ILogger logger, string singletonGuid)
    {
        var stopThread = false;
        
        app.MapGet("/home", () =>
        {
            logger.LogInformation($"trace: {singletonGuid}");
            
            return Results.Ok(new ApiResponse<string>
            {
                Data = "online",
                Success = true,
                Errors = null
            });
        });
        
        app.MapGet("/name", async (HttpRequest request, HttpContext context, IHttpClientFactory httpFactory) =>
        {
            var invertTextApiRequest = new InvertTextApiRequest();
            var clientRequest = new ClientRequest<InvertTextApiRequest>(invertTextApiRequest, httpFactory, logger);
            InvertTextApiResponse response = await clientRequest.GetAsync();
            
            var headers = request.Headers;
            var correlationId = headers["CorrelationId"].FirstOrDefault();
        
            if (string.IsNullOrEmpty(correlationId))
                correlationId = Guid.NewGuid().ToString();
        
            context.Response.Headers.Append("correlationId", correlationId);

            var traceId = Activity.Current?.SetTag("correlation.id", correlationId).TraceId.ToString();
            logger.LogInformation("Request processed with correlationId: {CorrelationId}", correlationId);
            
            return Results.Ok(new ApiResponse<InvertTextApiResponse>()
            {
                Data = response,
                Success = true,
                Errors = null
            });
        });
        
        app.MapGet("/start-high-cpu/{numThreads}", (int numThreads) =>
        {
            stopThread = false;
            
            for (int i = 0; i < numThreads; i++)
            {
                var t = new Thread(() =>
                {
                    while (true)
                    {
                        var http = new HttpClient();
                        http.BaseAddress = new Uri("https://google.com");
                        var result = http.GetAsync("/").Result;
                        
                        logger.LogInformation("High CPU thread running... {0}",Thread.CurrentThread.ManagedThreadId);
                        if (stopThread) break;
                    }
                });
                
                t.Start();
            }
        });
        
        app.MapGet("/stop-high-cpu", () =>
        {
            stopThread = true;
        });
        
        app.MapGet("/sleep", (int time) =>
        {
            logger.LogInformation("Sleeping for {0} seconds...", time);
            Thread.Sleep(1000 * time);
            logger.LogInformation("Awake!");
        });
    }
}
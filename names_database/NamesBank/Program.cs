using NamesBank.DI;
using NamesBank.Models;

var builder = WebApplication.CreateBuilder(args);
var appName = "NamesBank";
var singletonGuid = Guid.NewGuid().ToString();

builder.AddOtelConfig(appName);

var conf = builder.Configuration.AddJsonFile("appsettings.Development.json",
    false, 
    true
).Build();

builder.Services.AddHttpClient(nameof(InvertTextApiRequest), client =>
{
    client.BaseAddress = new Uri("https://api.invertexto.com");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {conf["invertTextToken"]}");
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.MapEndpoints(logger, singletonGuid);

app.Run();
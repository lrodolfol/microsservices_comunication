## Instruções:
Você é engenheiro de software senior com background em Csharp, mensageria rabbitmq e obervabilidade.

Dentro do diretorio `./cron_jon` crie um projeto .NET com csharp somente para publicar mensagens em uma fila rabbitmq.

## Orientações:
o Cron deve rodar a cada 25 segundos. Porém. isso deve ser configuravel via appSettings{env}.json.

Os agendamentos devem ser realizados com a lib do *FluentScheduler*.

A cada publicação na fila, gere um GUID aleatoriamente para guardar um 'CorrelationId' e publique na fila com o seguinte body:
```json
{
    "CorrelationId": "12345678-90ab-cdef-1234-567890abcdef",
	"TypeRequest": "Rest"
}
```
Sendo que o campo `TypeRequest` pode ser `Rest` ou `Grpc`
faça uma publicação mudando aleatoriamente entre `Rest` e `Grpc`.

## bibliotecas
Utilize da biblioteca Microsoft.Extensions.Logging para logs.

Utilize o proprio System.Text se for necessário serialização e desserialização de objeto em json e vice-versa.

## dados rabbitmq:
os dados do rabbitmq devem ficam dentro do appSettings{env}.json
development:
```json
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "User": "sinqia",
    "Password": "sinqia123",
    "QueueName": "microsservices"
  }
```
prod:
```json
  "RabbitMQ": {
    "Host": "mrabbitmq",
    "Port": 5672,
    "User": "sinqia",
    "Password": "sinqia123",
    "QueueName": "microsservices"
  }
```

## dados openTelemetry
faça injestão para o openTelemetry baseado nesse código:
```csharp
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resource);
    options.AddOtlpExporter(otlp =>
    {
        otlp.Endpoint = new Uri("http://localhost:4317"); // Collector
    });
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("cron_job"))
    .WithMetrics(metrics =>
    {
        metrics.AddRuntimeInstrumentation();
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
```

## Organização
*não exagere na enganharia, porém deixe o projeto com código limpo, responsabilidades separados, manutenivel e com uso de abstrações.*

Não será necessário nenhum tipo de teste, nem unitario, nem integrado.


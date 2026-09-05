using NamesBank.Models;
using NSubstitute;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace NamesBank.Tests;

public class NameServerApiMock : IDisposable
{
    private readonly WireMockServer _wireMockServer;
    public string Url => _wireMockServer.Url!;
    
    public NameServerApiMock() => _wireMockServer = WireMockServer.Start();
    
    public void SetupValidMock()
    {
        _wireMockServer.Given(Request
                .Create()
                .UsingGet()
                .WithPath(new InvertTextApiRequest().EndPoint)
            )
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBodyAsJson(new InvertTextApiResponse()
                {
                    Cpf = "123.456.789-00",
                    Email = "myUser@email.com",
                    Name = "Bruce Wayne"
                })
            );
    }
    
    public void SetupInvalidMock()
    {
        _wireMockServer.Given(Request
                .Create()
                .UsingGet()
                .WithPath($"{new InvertTextApiRequest().EndPoint}-invalid-path")
            )
            .RespondWith(Response.Create()
                .WithStatusCode(500)
            );
    }


    public void Dispose()
    {
        _wireMockServer.Stop();
        _wireMockServer.Dispose();
    }
}
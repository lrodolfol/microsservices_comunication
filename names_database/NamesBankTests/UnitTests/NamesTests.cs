using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NamesBank.Gateways;
using NamesBank.Models;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace NamesBank.Tests;

public class NamesTests
{
    private readonly NameServerApiMock _nameServerApiMock;

    public NamesTests()
    {
        _nameServerApiMock = new NameServerApiMock();
    }
    
    [Fact(DisplayName = nameof(Should_return_model_with_valid_request))]
    public async Task Should_return_model_with_valid_request()
    {
        _nameServerApiMock.SetupValidMock();
        
        var httpClient = new HttpClient() { BaseAddress = new Uri(_nameServerApiMock.Url) };
        
        var httpFactory = Substitute.For<IHttpClientFactory>();
        httpFactory.CreateClient(nameof(InvertTextApiRequest)).Returns(httpClient);
        
        var logger = Substitute.For<ILogger>();

        var invertText = new InvertTextApi();
        var invertTextRequest = new InvertTextApiRequest();
        var client = new ClientRequest<InvertTextApiRequest>(invertTextRequest, httpFactory, logger);

        var result = await client.GetAsync();

        invertText.Request = invertTextRequest;
        invertText.Response = result;
        
        Assert.True(invertText.Response.Name is not null);
        Assert.True(invertText.Response.Cpf is not null);
        Assert.True(invertText.Response.Email is not null);
        Assert.NotNull(invertText.Response);
    }
    
    [Fact(DisplayName = nameof(Should_return_model_with_invalid_request))]
    public async Task Should_return_model_with_invalid_request()
    {
        _nameServerApiMock.SetupInvalidMock();
        
        var httpClient = new HttpClient() { BaseAddress = new Uri(_nameServerApiMock.Url) };
        
        var httpFactory = Substitute.For<IHttpClientFactory>();
        httpFactory.CreateClient(nameof(InvertTextApiRequest)).Returns(httpClient);
        
        var logger = Substitute.For<ILogger>();
        
        var invertText = new InvertTextApiRequest();
        var client = new ClientRequest<InvertTextApiRequest>(invertText, httpFactory, logger);

        var result = await client.GetAsync();
        
        Assert.True(result.Name is not null);
        Assert.True(result.Cpf is not null);
        Assert.True(result.Email is not null);
        Assert.True(result is InvertTextApiResponse);
    }

    [Fact(DisplayName = nameof(Should_intanciate_name))]
    public void Should_intanciate_name()
    {
        var name = new Name("Bruce Wayne");
        
        Assert.Equal("Bruce Wayne", name.Value);
    }
    
    [Fact(DisplayName = nameof(ForceError))]
    public void ForceError()
    {
        Assert.True(true);
    }
}

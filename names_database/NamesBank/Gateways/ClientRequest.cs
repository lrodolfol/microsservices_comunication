using Bogus;
using Bogus.Extensions.Brazil;
using NamesBank.Contracts;
using NamesBank.Models;

namespace NamesBank.Gateways;

public class ClientRequest<T> where T : ApiEndPoint
{
    private readonly HttpClient _client;
    private readonly T _model;
    private readonly ILogger _logger;

    public ClientRequest(T model, IHttpClientFactory httpFactory, ILogger logger)
    {
        _model = model;
        _client = httpFactory.CreateClient(typeof(T).Name);
        _logger = logger;
    }
    
    public async Task<InvertTextApiResponse> GetAsync()
    {
        //return await GetHttp() ?? GetMocked();
        return GetMocked();
    }

    private async Task<InvertTextApiResponse?> GetHttp()
    {
        var requestResponse = await _client.GetAsync(_model.EndPoint);
        if (!requestResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("Error fetching from InvertText API: {StatusCode} - {ReasonPhrase}. Using mocked data instead."
                , requestResponse.StatusCode, requestResponse.ReasonPhrase);
            return null;   
        }
        
        var modelResponse = await requestResponse.Content.ReadFromJsonAsync<InvertTextApiResponse>();
        return modelResponse;
    }

    private InvertTextApiResponse GetMocked()
    {
        var bogus = new Faker<InvertTextApiResponse>()
            .RuleFor(x => x.Cpf, f => f.Person.Cpf())
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .RuleFor(x => x.Email, f => f.Person.Email);
        
        var model = bogus.Generate() ?? new InvertTextApiResponse();

        return model;
    }
}
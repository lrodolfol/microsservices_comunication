using NamesBank.Contracts;

namespace NamesBank.Models;

public class InvertTextApi
{
    public InvertTextApiRequest Request { get; set; } = null!;
    public InvertTextApiResponse Response { get; set; } = null!;
}
public class InvertTextApiRequest() : ApiEndPoint("/v1/faker");
public class InvertTextApiResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
}
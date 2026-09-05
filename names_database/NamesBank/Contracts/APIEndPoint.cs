namespace NamesBank.Contracts;

public abstract class ApiEndPoint(string endPoint)
{
    public string EndPoint { get; set; } = endPoint;
}
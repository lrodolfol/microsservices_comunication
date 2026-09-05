namespace NamesBank.Models;

public class Name(string value)
{
    public string Value { get; private set; } = value;
}
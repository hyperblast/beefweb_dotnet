namespace Beefweb.CommandLineTool.Services;

public readonly record struct Token(string Value, int Line, int Offset)
{
    public string Location => $"{Line}:{Offset}";
}

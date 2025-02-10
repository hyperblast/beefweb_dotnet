namespace Beefweb.Client.Infrastructure;

internal sealed class ErrorResponse
{
    public ErrorInfo? Error { get; set; }
}

internal sealed class ErrorInfo
{
    public string? Message { get; set; }

    public string? Parameter { get; set; }
}

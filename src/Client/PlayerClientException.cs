using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Beefweb.Client;

/// <summary>
/// Exception for failed HTTP requests with context information.
/// </summary>
public class PlayerClientException : HttpRequestException
{
    /// <summary>
    /// Server error message.
    /// </summary>
    public string? ServerErrorMessage { get; }

    /// <summary>
    /// Name of the parameter which caused error.
    /// </summary>
    public string? ErrorParameterName { get; }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="requestError">Error type.</param>
    /// <param name="statusCode">Status code.</param>
    /// <param name="serverErrorMessage">Server error message.</param>
    /// <param name="errorParameterName">Error parameter name.</param>
    /// <param name="inner">Inner exception</param>
    public PlayerClientException(
        string message,
        HttpRequestError requestError = HttpRequestError.Unknown,
        HttpStatusCode? statusCode = null,
        string? serverErrorMessage = null,
        string? errorParameterName = null,
        Exception? inner = null)
        : base(requestError, message, inner, statusCode)
    {
        ServerErrorMessage = serverErrorMessage;
        ErrorParameterName = errorParameterName;
    }

    internal static PlayerClientException Create(
        HttpStatusCode statusCode,
        string? reasonPhrase = null,
        string? serverErrorMessage = null,
        string? errorParameterName = null)
    {
        var messageBuilder = new StringBuilder(150);

        messageBuilder.Append(
            CultureInfo.InvariantCulture,
            $"Response status code does not indicate success: {(int)statusCode}");

        if (reasonPhrase != null)
        {
            messageBuilder.Append($" ({reasonPhrase})");
        }

        messageBuilder.Append('.');

        if (serverErrorMessage != null)
        {
            messageBuilder.Append(" Server error: ").Append(serverErrorMessage);

            if (!serverErrorMessage.EndsWith('.'))
                messageBuilder.Append('.');
        }

        if (errorParameterName != null)
        {
            messageBuilder.Append($" Parameter name: {errorParameterName}.");
        }

        return new PlayerClientException(
            messageBuilder.ToString(),
            HttpRequestError.Unknown,
            statusCode,
            serverErrorMessage,
            errorParameterName);
    }
}

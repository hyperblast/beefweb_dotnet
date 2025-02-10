using System;
using System.Net;
using System.Net.Http;

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
}

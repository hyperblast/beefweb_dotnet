﻿using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure;

internal sealed class RequestHandler : IRequestHandler
{
    private static readonly Encoding Utf8 = new UTF8Encoding(false);
    private static readonly Type RawJsonType = typeof(RawJson);
    private static readonly Type ErrorResponseType = typeof(ErrorResponse);
    private static readonly byte[] EventPrefix = "data:"u8.ToArray();

    internal static readonly JsonSerializerOptions DefaultSerializerOptions = CreateSerializerOptions();

    private readonly HttpClient _client;
    private readonly Uri _baseUri;
    private readonly AuthenticationHeaderValue? _authHeader;
    private readonly ILineReaderFactory _readerFactory;

    public RequestHandler(HttpClient client, Uri baseUri, ApiCredentials? credentials, ILineReaderFactory readerFactory)
    {
        _client = client;
        _baseUri = UriFormatter.AddTrailingSlash(baseUri);
        _authHeader = GetAuthHeader(credentials);
        _readerFactory = readerFactory;
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var namingPolicy = JsonNamingPolicy.CamelCase;

        return new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = namingPolicy,
            Converters =
            {
                new UnixTimestampConverter(),
                new TimeSpanAsSecondsConverter(),
                new FileSystemEntryTypeConverter(),
                new PlaylistRefConverter(),
                new JsonStringEnumConverter(namingPolicy),
            },
        };
    }

    public async ValueTask<object?> Get(
        Type returnType,
        string url,
        QueryParameterCollection? queryParams = null,
        JsonSerializerOptions? serializerOptions = null,
        bool allowNullResponse = false,
        CancellationToken cancellationToken = default)
    {
        var requestUri = UriFormatter.Format(_baseUri, url, queryParams);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypes.Json));
        request.Headers.Authorization = _authHeader;

        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            await ThrowResponseError(response, cancellationToken).ConfigureAwait(false);

        return await ParseResponse(response, returnType, serializerOptions, allowNullResponse, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<IStreamedResult?> GetStream(string url, QueryParameterCollection? queryParams = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = UriFormatter.Format(_baseUri, url, queryParams);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = _authHeader;

        var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            response.Dispose();
            return null;
        }

        if (response.IsSuccessStatusCode)
            return new HttpStreamedResult(response);

        using (response)
        {
            await ThrowResponseError(response, cancellationToken).ConfigureAwait(false);
            return null;
        }
    }

    public async IAsyncEnumerable<object> GetEvents(
        Type itemType,
        string url,
        QueryParameterCollection? queryParams = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestUri = UriFormatter.Format(_baseUri, url, queryParams);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypes.EventStream));
        request.Headers.Authorization = _authHeader;

        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            await ThrowResponseError(response, cancellationToken).ConfigureAwait(false);

        var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await using var responseStreamScope = responseStream.ConfigureAwait(false);

        await foreach (var lineData in _readerFactory.CreateReader(responseStream).ConfigureAwait(false))
        {
            // Note: this does not support multi-line events, see spec:
            // https://html.spec.whatwg.org/multipage/server-sent-events.html#event-stream-interpretation

            if (lineData.Span.StartsWith(EventPrefix))
            {
                var eventValue = JsonSerializer.Deserialize(
                    lineData.Span[EventPrefix.Length..], itemType, DefaultSerializerOptions);
                yield return eventValue ?? throw InvalidResponse();
            }
        }
    }

    public async ValueTask<object?> Post(
        Type? returnType, string url, object? body = null, JsonSerializerOptions? serializerOptions = null,
        bool allowNullResponse = false, CancellationToken cancellationToken = default)
    {
        var requestUri = UriFormatter.Format(_baseUri, url);
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Authorization = _authHeader;

        if (returnType != null)
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypes.Json));

        request.Content = GetContent();

        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            await ThrowResponseError(response, cancellationToken).ConfigureAwait(false);

        return returnType != null
            ? await ParseResponse(response, returnType, serializerOptions, allowNullResponse, cancellationToken)
                .ConfigureAwait(false)
            : null;

        HttpContent GetContent()
        {
            if (body is RawJson rawJson)
            {
                return new StringContent(rawJson.Value ?? "null", Utf8,
                    new MediaTypeHeaderValue(ContentTypes.Json, "utf-8"));
            }

            if (body != null)
            {
                return JsonContent.Create(body, body.GetType(), options: serializerOptions ?? DefaultSerializerOptions);
            }

            return new ByteArrayContent([]);
        }
    }

    private static async ValueTask<object?> ParseResponse(HttpResponseMessage response, Type returnType,
        JsonSerializerOptions? serializerOptions, bool allowNull, CancellationToken cancellationToken)
    {
        if (returnType == RawJsonType)
        {
            return new RawJson(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
        }

        var result = await response.Content
            .ReadFromJsonAsync(returnType, serializerOptions ?? DefaultSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        if (result == null && !allowNull)
        {
            throw InvalidResponse();
        }

        return result;
    }

    private static async ValueTask ThrowResponseError(
        HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentType?.MediaType != ContentTypes.Json)
        {
            throw PlayerClientException.Create(response.StatusCode, response.ReasonPhrase);
        }

        var errorResponse = (ErrorResponse?)await ParseResponse(
                response, ErrorResponseType, DefaultSerializerOptions, true, cancellationToken)
            .ConfigureAwait(false);

        throw PlayerClientException.Create(
            response.StatusCode,
            response.ReasonPhrase,
            errorResponse?.Error?.Message,
            errorResponse?.Error?.Parameter);
    }

    private static AuthenticationHeaderValue? GetAuthHeader(ApiCredentials? credentials)
    {
        if (credentials == null || credentials.IsEmpty)
            return null;

        var parameter = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(credentials.UserName + ":" + credentials.Password));

        return new AuthenticationHeaderValue("Basic", parameter);
    }

    private static PlayerClientException InvalidResponse()
    {
        return new PlayerClientException("Invalid response: expected JSON object.", HttpRequestError.InvalidResponse);
    }
}

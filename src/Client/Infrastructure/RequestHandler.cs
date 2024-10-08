﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure;

internal sealed class RequestHandler : IRequestHandler
{
    private static readonly byte[] EventPrefix = "data:"u8.ToArray();

    internal static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    private readonly HttpClient _client;
    private readonly ILineReaderFactory _readerFactory;
    private readonly Uri _baseUri;

    public RequestHandler(Uri baseUri, HttpClient client, ILineReaderFactory readerFactory)
    {
        _baseUri = UriFormatter.AddTrailingSlash(baseUri);
        _client = client;
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
                new JsonStringEnumConverter(namingPolicy)
            },
        };
    }

    public async ValueTask<object> Get(
        Type returnType,
        string url,
        QueryParameterCollection? queryParams = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = UriFormatter.Format(_baseUri, url, queryParams);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypes.Json));

        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await using var responseStreamScope = responseStream.ConfigureAwait(false);

        var result = await JsonSerializer
            .DeserializeAsync(responseStream, returnType, SerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        return result ?? throw InvalidResponse();
    }

    private static InvalidOperationException InvalidResponse()
    {
        return new InvalidOperationException("Invalid response: expected JSON value.");
    }

    public async ValueTask<IStreamedResult?> GetStream(string url, QueryParameterCollection? queryParams = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = UriFormatter.Format(_baseUri, url, queryParams);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            response.Dispose();
            return null;
        }

        try
        {
            response.EnsureSuccessStatusCode();
            return new HttpStreamedResult(response);
        }
        catch
        {
            response.Dispose();
            throw;
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

        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await using var responseStreamScope = responseStream.ConfigureAwait(false);

        await foreach (var lineData in _readerFactory.CreateReader(responseStream).ConfigureAwait(false))
        {
            // Note: this does not support multi-line events, see spec:
            // https://html.spec.whatwg.org/multipage/server-sent-events.html#event-stream-interpretation

            if (lineData.Span.StartsWith(EventPrefix))
            {
                var eventValue = JsonSerializer.Deserialize(
                    lineData.Span[EventPrefix.Length..], itemType, SerializerOptions);
                yield return eventValue ?? throw InvalidResponse();
            }
        }
    }

    public async ValueTask Post(string url, object? body = null, CancellationToken cancellationToken = default)
    {
        var requestUri = UriFormatter.Format(_baseUri, url);
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

        request.Content = body != null
            ? CreateContent(ContentTypes.Json, JsonSerializer.SerializeToUtf8Bytes(body, SerializerOptions))
            : CreateContent(ContentTypes.Text, []);

        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        return;

        static ByteArrayContent CreateContent(string type, byte[] data) =>
            new(data) { Headers = { ContentType = new MediaTypeHeaderValue(type) } };
    }
}

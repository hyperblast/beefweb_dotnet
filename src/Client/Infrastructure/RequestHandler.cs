using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure
{
    public sealed class RequestHandler : IRequestHandler
    {
        private static readonly byte[] EventPrefix =
        {
            (byte)'d', (byte)'a', (byte)'t', (byte)'a', (byte)':'
        };

        private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

        private readonly HttpClient _client;
        private readonly ILineReaderFactory _readerFactory;
        private readonly Uri _baseUri;

        public RequestHandler(Uri baseUri, HttpClient client, ILineReaderFactory readerFactory)
        {
            _baseUri = UriFormatter.AddTrailingSlash(baseUri);
            _client = client;
            _readerFactory = readerFactory;
        }

        public static JsonSerializerOptions CreateSerializerOptions()
        {
            var namingPolicy = JsonNamingPolicy.CamelCase;

            return new JsonSerializerOptions
            {
                IgnoreNullValues = true,
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
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
                Headers = { Accept = { new MediaTypeWithQualityHeaderValue(ContentTypes.Json) } },
            };

            using var response = await _client.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync(
                responseStream, returnType, SerializerOptions, cancellationToken: cancellationToken);
        }

        public async IAsyncEnumerable<object> GetEvents(
            Type itemType,
            string url,
            QueryParameterCollection? queryParams = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var requestUri = UriFormatter.Format(_baseUri, url, queryParams);
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
                Headers = { Accept = { new MediaTypeWithQualityHeaderValue(ContentTypes.EventStream) } }
            };

            using var response = await _client.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            await foreach (var lineData in _readerFactory.CreateReader(responseStream))
            {
                // Note: this does not support multi-line events, see spec:
                // https://html.spec.whatwg.org/multipage/server-sent-events.html#event-stream-interpretation

                if (lineData.Span.StartsWith(EventPrefix))
                {
                    yield return JsonSerializer.Deserialize(
                        lineData.Span[EventPrefix.Length..], itemType, SerializerOptions);
                }
            }
        }

        public async ValueTask Post(string url, object? body = null, CancellationToken cancellationToken = default)
        {
            static ByteArrayContent CreateContent(string type, byte[] data) =>
                new ByteArrayContent(data) { Headers = { ContentType = new MediaTypeHeaderValue(type) } };

            var requestUri = UriFormatter.Format(_baseUri, url);
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = body != null
                    ? CreateContent(ContentTypes.Json, JsonSerializer.SerializeToUtf8Bytes(body, SerializerOptions))
                    : CreateContent(ContentTypes.Text, Array.Empty<byte>())
            };

            using var response =
                await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }
}
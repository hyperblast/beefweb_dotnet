using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure
{
    public sealed class HttpStreamedResult : IStreamedResult
    {
        private readonly HttpResponseMessage _message;

        public string ContentType => _message.Content.Headers.ContentType.MediaType;

        public long? Size => _message.Content.Headers.ContentLength;

        public HttpStreamedResult(HttpResponseMessage message) => _message = message;

        public async ValueTask<Stream> GetStream(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _message.Content.ReadAsStreamAsync();
        }

        public void Dispose() => _message.Dispose();
    }
}

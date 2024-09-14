using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure;

internal sealed class HttpStreamedResult(HttpResponseMessage message) : IStreamedResult
{
    public string ContentType => message.Content.Headers.ContentType?.MediaType ?? ContentTypes.Binary;

    public long? Size => message.Content.Headers.ContentLength;

    public async ValueTask<Stream> GetStream(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await message.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Dispose() => message.Dispose();
}

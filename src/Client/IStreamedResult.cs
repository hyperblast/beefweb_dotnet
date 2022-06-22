using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client
{
    public interface IStreamedResult : IDisposable
    {
        string ContentType { get; }

        long? Size { get; }

        ValueTask<Stream> GetStream(CancellationToken cancellationToken = default);
    }
}
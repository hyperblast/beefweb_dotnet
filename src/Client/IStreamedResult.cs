using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client;

/// <summary>
/// Streamed result. Used for returning artwork data.
/// </summary>
public interface IStreamedResult : IDisposable
{
    /// <summary>
    /// Result content type.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Result content size.
    /// </summary>
    long? Size { get; }

    /// <summary>
    /// Reads content and returns content <see cref="Stream"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Content stream.</returns>
    ValueTask<Stream> GetStream(CancellationToken cancellationToken = default);
}
using System.IO;

namespace Beefweb.Client.Infrastructure;

internal sealed class LineReaderFactory(IGrowableBufferFactory bufferFactory) : ILineReaderFactory
{
    public ILineReader CreateReader(Stream source) => new LineReader(source, bufferFactory);
}

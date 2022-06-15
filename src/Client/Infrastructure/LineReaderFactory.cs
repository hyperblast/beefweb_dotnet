using System.IO;

namespace Beefweb.Client.Infrastructure
{
    public sealed class LineReaderFactory : ILineReaderFactory
    {
        private readonly IGrowableBufferFactory _bufferFactory;

        public LineReaderFactory(IGrowableBufferFactory bufferFactory) => _bufferFactory = bufferFactory;

        public ILineReader CreateReader(Stream source) => new LineReader(source, _bufferFactory);
    }
}
using System.IO;

namespace Beefweb.Client.Infrastructure
{
    public interface ILineReaderFactory
    {
        ILineReader CreateReader(Stream source);
    }
}
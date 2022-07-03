using System.IO;

namespace Beefweb.Client.Infrastructure
{
    internal interface ILineReaderFactory
    {
        ILineReader CreateReader(Stream source);
    }
}

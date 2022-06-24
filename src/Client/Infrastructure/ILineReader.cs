using System;
using System.Collections.Generic;

namespace Beefweb.Client.Infrastructure
{
    public interface ILineReader : IAsyncEnumerable<ReadOnlyMemory<byte>>
    {
    }
}

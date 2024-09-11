using System;
using System.Collections.Generic;

namespace Beefweb.Client.Infrastructure;

internal interface ILineReader : IAsyncEnumerable<ReadOnlyMemory<byte>>;

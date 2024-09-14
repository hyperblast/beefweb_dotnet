using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Beefweb.Client.Infrastructure;

internal sealed class LineReader(Stream source, IGrowableBufferFactory bufferFactory) : ILineReader
{
    private const byte CR = (byte)'\r';
    private const byte LF = (byte)'\n';

    public async IAsyncEnumerator<ReadOnlyMemory<byte>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        using var buffer = bufferFactory.CreateBuffer();
        var headSize = 0;
        var prevSeparator = (byte)0;

        while (true)
        {
            var block = buffer.Data.AsMemory(headSize, buffer.Data.Length - headSize);
            var blockSize = await source.ReadAsync(block, cancellationToken).ConfigureAwait(false);
            if (blockSize == 0)
                yield break;

            var totalSize = headSize + blockSize;
            var prevSeparatorIndex = -1;

            for (var i = headSize; i < totalSize; i++)
            {
                var value = buffer.Data[i];
                if (value != CR && value != LF)
                    continue;

                var length = i - prevSeparatorIndex - 1;
                if (value == LF && prevSeparator == CR && length == 0)
                {
                    // LF that is preceded by CR -> ignore the line
                }
                else
                {
                    yield return buffer.Data.AsMemory(prevSeparatorIndex + 1, length);
                }

                prevSeparator = value;
                prevSeparatorIndex = i;
            }

            if (prevSeparatorIndex < 0)
            {
                headSize += blockSize;

                if (headSize == buffer.Data.Length)
                    buffer.Grow();

                continue;
            }

            var tailSize = totalSize - prevSeparatorIndex - 1;
            if (tailSize > 0)
                Buffer.BlockCopy(buffer.Data, prevSeparatorIndex + 1, buffer.Data, 0, tailSize);

            headSize = tailSize;
        }
    }
}

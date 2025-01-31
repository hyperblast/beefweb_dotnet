using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.CommandLineTool.Services;

public static class TokenReader
{
    public static IAsyncEnumerable<Token> ReadTokensAsync(this TextReader reader) => ReadTokensImplAsync(reader);

    private static async IAsyncEnumerable<Token> ReadTokensImplAsync(
        TextReader reader, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var currentChar = ' ';
        var currentLine = 1;
        var currentOffset = 0;
        var tokenValue = new StringBuilder();
        var tokenLine = 0;
        var tokenOffset = 0;
        var bufferOffset = 0;
        var bufferRemaining = 0;
        var buffer = ArrayPool<char>.Shared.Rent(1024);

        try
        {
            while (await ReadNext())
            {
                if (char.IsWhiteSpace(currentChar) || char.IsControl(currentChar))
                {
                    if (tokenValue.Length > 0)
                    {
                        yield return new Token(tokenValue.ToString(), tokenLine, tokenOffset);
                        tokenValue.Clear();
                    }

                    continue;
                }

                tokenValue.Append(currentChar);

                if (tokenValue.Length == 1)
                {
                    tokenLine = currentLine;
                    tokenOffset = currentOffset;
                }
            }

            if (tokenValue.Length > 0)
            {
                yield return new Token(tokenValue.ToString(), tokenLine, tokenOffset);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }

        yield break;

        async ValueTask<bool> ReadNext()
        {
            ct.ThrowIfCancellationRequested();

            if (bufferRemaining == 0)
            {
                bufferOffset = 0;
                bufferRemaining = await reader.ReadBlockAsync(buffer, 0, buffer.Length);

                if (bufferRemaining == 0)
                {
                    return false;
                }
            }

            currentChar = buffer[bufferOffset];
            bufferOffset++;
            bufferRemaining--;

            if (currentChar is '\n')
            {
                currentLine++;
                currentOffset = 0;
            }
            else
            {
                currentOffset++;
            }

            return true;
        }
    }
}

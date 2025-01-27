using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.CommandLineTool.Services;

public static class Extensions
{
    public static IAsyncEnumerable<int> ReadIndicesAsync(this TextReader reader) => ReadIndicesImplAsync(reader);

    private static async IAsyncEnumerable<int> ReadIndicesImplAsync(
        TextReader reader, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var line = 0;
        var offset = -1;
        var ch = ' ';
        var value = -1;
        var bufferOffset = 0;
        var bufferRemaining = 0;
        var buffer = ArrayPool<char>.Shared.Rent(1024);

        try
        {
            while (await ReadNext())
            {
                if (char.IsWhiteSpace(ch) || char.IsControl(ch))
                {
                    if (value >= 0)
                    {
                        yield return value;
                        value = -1;
                    }

                    continue;
                }

                if (!char.IsAsciiDigit(ch))
                {
                    throw new InvalidRequestException($"Invalid input character '{ch}' at {line}:{offset}.");
                }

                var digit = ch - '0';
                value = value >= 0 ? value * 10 + digit : digit;

                if (value > 100_000_000)
                {
                    throw new InvalidRequestException($"Value {value} is too large at {line}:{offset}.");
                }
            }

            if (value >= 0)
            {
                yield return value;
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

            ch = buffer[bufferOffset];
            bufferOffset++;
            bufferRemaining--;

            if (ch is '\n')
            {
                line++;
                offset = -1;
            }
            else
            {
                offset++;
            }

            return true;
        }
    }

    public static IAsyncEnumerable<string> ReadLinesAsync(this TextReader reader) => ReadLinesImplAsync(reader);

    private static async IAsyncEnumerable<string> ReadLinesImplAsync(
        TextReader reader, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var line = await reader.ReadLineAsync(ct);
        while (line != null)
        {
            yield return line;
            line = await reader.ReadLineAsync(ct);
        }
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
            collection.Add(item);
    }

    public static async ValueTask AddRangeAsync<T>(
        this ICollection<T> collection, IAsyncEnumerable<T> items, CancellationToken ct = default)
    {
        await foreach (var item in items.WithCancellation(ct))
            collection.Add(item);
    }

    public static async ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items,
        CancellationToken ct = default)
    {
        var result = new List<T>();

        await foreach (var item in items.WithCancellation(ct))
            result.Add(item);

        return result;
    }

    public static IReadOnlyList<string> GetOrDefault(
        this IReadOnlyList<string>? values, IReadOnlyList<string> defaultValues)
    {
        return values is { Count: > 0 } ? values : defaultValues;
    }
}

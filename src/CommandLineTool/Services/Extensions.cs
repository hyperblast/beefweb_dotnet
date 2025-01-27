using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Beefweb.CommandLineTool.Services;

public static class Extensions
{
    public static IEnumerable<int> ReadIndices(this TextReader reader, CancellationToken ct = default)
    {
        var line = 0;
        var offset = 0;

        var input = reader.Read();
        var value = -1;

        while (input != -1)
        {
            var ch = (char)input;

            if (char.IsWhiteSpace(ch) || char.IsControl(ch))
            {
                if (value >= 0)
                {
                    yield return value;
                    value = -1;
                }

                ReadNext();
                continue;
            }

            if (!char.IsAsciiDigit(ch))
            {
                throw new InvalidRequestException($"Invalid input character '{ch}' at {line}:{offset}.");
            }

            var digit = input - '0';
            value = value >= 0 ? value * 10 + digit : digit;

            if (value > 100_000_000)
            {
                throw new InvalidRequestException($"Value {value} is too large at {line}:{offset}.");
            }

            ReadNext();
        }

        if (value >= 0)
        {
            yield return value;
        }

        yield break;

        void ReadNext()
        {
            ct.ThrowIfCancellationRequested();

            input = reader.Read();

            if (input is '\n' or '\r')
            {
                line++;
                offset = 0;
            }
            else
            {
                offset++;
            }
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

    public static IReadOnlyList<string> GetOrDefault(
        this IReadOnlyList<string>? values, IReadOnlyList<string> defaultValues)
    {
        return values is { Count: > 0 } ? values : defaultValues;
    }
}

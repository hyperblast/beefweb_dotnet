using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Beefweb.CommandLineTool.Services;

public static class Extensions
{
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

    public static IReadOnlyList<string> GetOrDefault(
        this IReadOnlyList<string>? values, IReadOnlyList<string> defaultValues)
    {
        return values is { Count: > 0 } ? values : defaultValues;
    }
}

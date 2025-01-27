using System.Collections.Generic;

namespace Beefweb.CommandLineTool.Services;

public static class Extensions
{
    public static IReadOnlyList<string> GetOrDefault(
        this IReadOnlyList<string>? values, IReadOnlyList<string> defaultValues)
    {
        return values is { Count: > 0 } ? values : defaultValues;
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Beefweb.CommandLineTool.Services;

public readonly record struct Range(int Start, int Count)
{
    public IEnumerable<int> GetItems(int totalCount)
    {
        var count = Count >= 0 ? Count : totalCount - Start;
        return count > 0 ? Enumerable.Range(Start, count) : [];
    }

    public static bool TryParse(ReadOnlySpan<char> input, out Range result)
    {
        var sep = input.IndexOf(':');
        if (sep < 0)
        {
            if (!int.TryParse(input, CultureInfo.InvariantCulture, out var index) || index < 0)
            {
                result = default;
                return false;
            }

            result = new Range(index, 1);
            return true;
        }

        var first = input[..sep];
        var second = input[(sep + 1)..];

        if (!int.TryParse(first, CultureInfo.InvariantCulture, out var start) || start < 0)
        {
            result = default;
            return false;
        }

        if (sep == input.Length - 1)
        {
            result = new Range(start, -1);
            return true;
        }

        if (!int.TryParse(second, CultureInfo.InvariantCulture, out var count) || count < 0)
        {
            result = default;
            return false;
        }

        result = new Range(start, count);
        return true;
    }
}

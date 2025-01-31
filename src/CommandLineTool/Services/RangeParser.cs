using System;

namespace Beefweb.CommandLineTool.Services;

public static class RangeParser
{
    public static bool TryParse(ReadOnlySpan<char> input, bool zeroBased, out Range range)
    {
        const string separator = "..";

        var sep = input.IndexOf(separator, StringComparison.Ordinal);
        if (sep < 0)
        {
            if (!IndexParser.TryParse(input, zeroBased, out var index))
            {
                range = default;
                return false;
            }

            range = new Range(index, index);
            return true;
        }

        var fromStr = input[..sep];
        var toStr = input[(sep + separator.Length)..];

        Index fromIndex;
        Index toIndex;

        if (fromStr.Length == 0)
        {
            fromIndex = Index.FromStart(0);
        }
        else if (!IndexParser.TryParse(fromStr, zeroBased, out fromIndex))
        {
            range = default;
            return false;
        }

        if (toStr.Length == 0)
        {
            toIndex = Index.FromEnd(0);
        }
        else if (!IndexParser.TryParse(toStr, zeroBased, out toIndex))
        {
            range = default;
            return false;
        }

        range = new Range(fromIndex, toIndex);
        return true;
    }

    public static Range Parse(ReadOnlySpan<char> input, bool zeroBased)
    {
        if (TryParse(input, zeroBased, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid item index or range '{input}'.");
    }

    public static Range Parse(Token input, bool zeroBased)
    {
        if (TryParse(input.Value, zeroBased, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid item index or range '{input}' at {input.Location}.");
    }
}

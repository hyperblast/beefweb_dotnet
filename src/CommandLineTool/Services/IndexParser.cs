using System;

namespace Beefweb.CommandLineTool.Services;

public static class IndexParser
{
    public static bool TryParse(ReadOnlySpan<char> input, bool zeroBased, out Index index)
    {
        const string minusZero = "-0";

        if (input.SequenceEqual(minusZero))
        {
            if (zeroBased)
            {
                index = Index.FromEnd(0);
                return true;
            }

            index = default;
            return false;
        }

        if (!ValueParser.TryParseInt(input, out var value))
        {
            index = default;
            return false;
        }

        if (!zeroBased && value == 0)
        {
            index = default;
            return false;
        }

        index = new Index(Math.Abs(value) + (zeroBased ? 0 : -1), value < 0);
        return true;
    }

    public static Index Parse(ReadOnlySpan<char> input, bool zeroBased)
    {
        if (TryParse(input, zeroBased, out var index))
        {
            return index;
        }

        throw new InvalidRequestException($"Invalid index value '{input.ToString()}'.");
    }

    public static int ParseAndGetOffset(string input, bool zeroBased, int totalCount)
    {
        return Parse(input, zeroBased).GetOffsetInclusive(totalCount);
    }
}

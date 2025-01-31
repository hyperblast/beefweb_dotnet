using System;
using System.Globalization;

namespace Beefweb.CommandLineTool.Services;

public static class PositionParser
{
    private static readonly (string name, TimeSpan value)[] DurationUnits;
    private static readonly string[] TimeSpanFormats;

    public static bool TryParse(ReadOnlySpan<char> input, out TimeSpan result)
    {
        if (input.Length == 0)
        {
            result = TimeSpan.Zero;
            return false;
        }

        double value;

        result = TimeSpan.Zero;

        foreach (var (unitName, unitValue) in DurationUnits)
        {
            if (!input.EndsWith(unitName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!ValueParser.TryParseDouble(input[..^unitName.Length], out value))
            {
                return false;
            }

            result = value * unitValue;
            return true;
        }

        if (ValueParser.TryParseDouble(input, out value))
        {
            result = TimeSpan.FromSeconds(value);
            return true;
        }

        if (input[0] == '-')
        {
            var s = input[1..];

            foreach (var format in TimeSpanFormats)
            {
                if (TimeSpan.TryParseExact(
                        s, format, CultureInfo.InvariantCulture, TimeSpanStyles.AssumeNegative, out result))
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (var format in TimeSpanFormats)
            {
                if (TimeSpan.TryParseExact(input, format, CultureInfo.InvariantCulture, out result))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static TimeSpan Parse(ReadOnlySpan<char> input)
    {
        if (TryParse(input, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid position value '{input}'.");
    }

    static PositionParser()
    {
        DurationUnits =
        [
            ("ms", TimeSpan.FromMilliseconds(1)),
            ("s", TimeSpan.FromSeconds(1)),
            ("m", TimeSpan.FromMinutes(1)),
            ("h", TimeSpan.FromHours(1)),
            ("d", TimeSpan.FromDays(1)),
        ];

        TimeSpanFormats =
        [
            @"m\:s",
            @"h\:m\:s",
            @"d\.h\:m\:s"
        ];
    }
}

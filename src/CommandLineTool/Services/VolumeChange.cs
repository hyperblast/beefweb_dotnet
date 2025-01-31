using System;

namespace Beefweb.CommandLineTool.Services;

public enum VolumeChangeType
{
    Linear = 0,
    Db = 1,
    Percent = 2,
}

public readonly record struct VolumeChange(VolumeChangeType Type, double Value)
{
    public static bool TryParse(ReadOnlySpan<char> input, out VolumeChange result)
    {
        const char percent = '%';
        const string db = "db";
        double value;

        if (input.Length == 0)
        {
            result = default;
            return false;
        }

        if (input[^1] == percent)
        {
            if (!ValueParser.TryParseDouble(input[..^1], out value))
            {
                result = default;
                return false;
            }

            result = new VolumeChange(VolumeChangeType.Percent, value);
            return true;
        }

        if (input.EndsWith(db, StringComparison.OrdinalIgnoreCase))
        {
            if (!ValueParser.TryParseDouble(input[..^db.Length], out value))
            {
                result = default;
                return false;
            }

            result = new VolumeChange(VolumeChangeType.Db, value);
            return true;
        }

        if (!ValueParser.TryParseDouble(input, out value))
        {
            result = default;
            return false;
        }

        result = new VolumeChange(VolumeChangeType.Linear, value);
        return true;
    }

    public static VolumeChange Parse(ReadOnlySpan<char> input)
    {
        if (TryParse(input, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid volume value '{input}', expected db, % or linear.");
    }
}

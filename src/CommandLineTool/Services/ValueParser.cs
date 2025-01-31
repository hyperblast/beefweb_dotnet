using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class ValueParser
{
    private static readonly FrozenDictionary<string, BoolSwitch> BoolSwitches;
    private static readonly FrozenDictionary<string, FileSystemEntryType> FsEntryTypes;
    private static readonly (string name, TimeSpan value)[] DurationUnits;
    private static readonly string[] TimeSpanFormats;

    private static bool TryParseInt(ReadOnlySpan<char> input, out int value)
    {
        return int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryParseDouble(ReadOnlySpan<char> input, out double value)
    {
        return double.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    public static bool TryParseIndex(ReadOnlySpan<char> input, bool zeroBased, out Index index)
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

        if (!TryParseInt(input, out var value))
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

    public static bool TryParseRange(ReadOnlySpan<char> input, bool zeroBased, out Range range)
    {
        const string separator = "..";

        var sep = input.IndexOf(separator, StringComparison.Ordinal);
        if (sep < 0)
        {
            if (!TryParseIndex(input, zeroBased, out var index))
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
        else if (!TryParseIndex(fromStr, zeroBased, out fromIndex))
        {
            range = default;
            return false;
        }

        if (toStr.Length == 0)
        {
            toIndex = Index.FromEnd(0);
        }
        else if (!TryParseIndex(toStr, zeroBased, out toIndex))
        {
            range = default;
            return false;
        }

        range = new Range(fromIndex, toIndex);
        return true;
    }

    public static bool TryParseVolumeChange(ReadOnlySpan<char> input, out VolumeChange result)
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
            if (!TryParseDouble(input[..^1], out value))
            {
                result = default;
                return false;
            }

            result = new VolumeChange(VolumeChangeType.Percent, value);
            return true;
        }

        if (input.EndsWith(db, StringComparison.OrdinalIgnoreCase))
        {
            if (!TryParseDouble(input[..^db.Length], out value))
            {
                result = default;
                return false;
            }

            result = new VolumeChange(VolumeChangeType.Db, value);
            return true;
        }

        if (!TryParseDouble(input, out value))
        {
            result = default;
            return false;
        }

        result = new VolumeChange(VolumeChangeType.Linear, value);
        return true;
    }

    public static bool TryParseTimeSpan(ReadOnlySpan<char> input, out TimeSpan result)
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

            if (!TryParseDouble(input[..^unitName.Length], out value))
            {
                return false;
            }

            result = value * unitValue;
            return true;
        }

        if (TryParseDouble(input, out value))
        {
            result = TimeSpan.FromSeconds(value);
            return true;
        }

        foreach (var format in TimeSpanFormats)
        {
            if (TimeSpan.TryParseExact(input, format, CultureInfo.InvariantCulture, out result))
            {
                return true;
            }
        }

        return false;
    }

    public static Index ParseIndex(ReadOnlySpan<char> input, bool zeroBased)
    {
        if (TryParseIndex(input, zeroBased, out var index))
        {
            return index;
        }

        throw new InvalidRequestException($"Invalid index value '{input.ToString()}'.");
    }

    public static int ParseOffset(string input, bool zeroBased, int totalCount)
    {
        return ParseIndex(input, zeroBased).GetOffsetInclusive(totalCount);
    }

    public static Range ParseRange(ReadOnlySpan<char> input, bool zeroBased)
    {
        if (TryParseRange(input, zeroBased, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid item index or range '{input}'.");
    }

    public static Range ParseRange(Token input, bool zeroBased)
    {
        if (TryParseRange(input.Value, zeroBased, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid item index or range '{input}' at {input.Location}.");
    }

    public static VolumeChange ParseVolumeChange(ReadOnlySpan<char> input)
    {
        if (TryParseVolumeChange(input, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid volume value '{input}', expected db, % or linear.");
    }

    public static TimeSpan ParseTimeSpan(ReadOnlySpan<char> input)
    {
        if (TryParseTimeSpan(input, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid position value '{input}'.");
    }

    public static double ParseDouble(ReadOnlySpan<char> input)
    {
        if (TryParseDouble(input, out var value))
        {
            return value;
        }

        throw new InvalidRequestException($"Invalid numeric value: {input}");
    }

    public static int ParseEnumValue(PlayerOption option, bool zeroBased, string input)
    {
        if (TryParseInt(input, out var displayIndex))
        {
            var realIndex = displayIndex + (zeroBased ? 0 : -1);
            if (realIndex < 0 || realIndex >= option.EnumNames!.Count)
            {
                throw new InvalidRequestException($"Option value index ({input}) is out of range.");
            }

            return realIndex;
        }

        var i = 0;
        foreach (var name in option.EnumNames!)
        {
            if (string.Equals(name, input, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }

            i++;
        }

        throw new InvalidRequestException($"Invalid option value '{input}'.");
    }

    public static BoolSwitch ParseSwitch(string input)
    {
        return BoolSwitches.TryGetValue(input, out var result)
            ? result
            : throw new InvalidRequestException($"Invalid bool value '{input}'.");
    }

    public static FileSystemEntryType ParseFileSystemEntryType(string input)
    {
        return FsEntryTypes.TryGetValue(input, out var result)
            ? result
            : throw new InvalidRequestException($"Invalid file system entry type '{input}'.");
    }

    static ValueParser()
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

        var boolSwitches = new Dictionary<string, BoolSwitch>(StringComparer.OrdinalIgnoreCase)
        {
            { "f", BoolSwitch.False },
            { "false", BoolSwitch.False },
            { "off", BoolSwitch.False },
            { "t", BoolSwitch.True },
            { "true", BoolSwitch.True },
            { "on", BoolSwitch.True },
            { "toggle", BoolSwitch.Toggle },
        };

        var fsEntryTypes = new Dictionary<string, FileSystemEntryType>(StringComparer.OrdinalIgnoreCase)
        {
            { "d", FileSystemEntryType.Directory },
            { "dir", FileSystemEntryType.Directory },
            { "directory", FileSystemEntryType.Directory },
            { "f", FileSystemEntryType.File },
            { "file", FileSystemEntryType.File }
        };

        BoolSwitches = boolSwitches.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        FsEntryTypes = fsEntryTypes.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}

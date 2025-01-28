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

    public static Range ParseRange(ReadOnlySpan<char> input)
    {
        if (Range.TryParse(input, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid item index or range '{input}'.");
    }

    public static Range ParseRange(Token input)
    {
        if (Range.TryParse(input.Value, out var result))
        {
            return result;
        }

        throw new InvalidRequestException($"Invalid item index or range '{input}' at {input.Location}.");
    }

    public static double ParseDouble(ReadOnlySpan<char> valueString)
    {
        if (double.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new InvalidRequestException($"Invalid numeric value: {valueString}");
    }

    public static int ParseEnumName(PlayerOption option, string value)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
        {
            if (index < 0 || index >= option.EnumNames!.Count)
            {
                throw new InvalidRequestException($"Option value index ({index}) is out of range.");
            }

            return index;
        }

        var i = 0;
        foreach (var name in option.EnumNames!)
        {
            if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }

            i++;
        }

        throw new InvalidRequestException($"Invalid option value '{value}'.");
    }

    public static BoolSwitch ParseSwitch(string value)
    {
        return BoolSwitches.TryGetValue(value, out var result)
            ? result
            : throw new InvalidRequestException($"Invalid bool value '{value}'.");
    }

    public static FileSystemEntryType ParseFileSystemEntryType(string value)
    {
        return FsEntryTypes.TryGetValue(value, out var result)
            ? result
            : throw new InvalidRequestException($"Invalid file system entry type '{value}'.");
    }

    static ValueParser()
    {
        var boolSwitches = new Dictionary<string, BoolSwitch>
        {
            { "f", BoolSwitch.False },
            { "false", BoolSwitch.False },
            { "off", BoolSwitch.False },
            { "t", BoolSwitch.True },
            { "true", BoolSwitch.True },
            { "on", BoolSwitch.True },
            { "toggle", BoolSwitch.Toggle },
        };

        var fsEntryTypes = new Dictionary<string, FileSystemEntryType>
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

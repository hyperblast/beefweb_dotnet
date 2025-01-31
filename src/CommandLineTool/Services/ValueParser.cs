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

    public static bool TryParseInt(ReadOnlySpan<char> input, out int value)
    {
        return int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    public static bool TryParseDouble(ReadOnlySpan<char> input, out double value)
    {
        return double.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
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

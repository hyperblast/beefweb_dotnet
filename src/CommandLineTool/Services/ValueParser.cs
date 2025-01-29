using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class ValueParser
{
    private static readonly FrozenDictionary<string, BoolSwitch> BoolSwitches;
    private static readonly FrozenDictionary<string, FileSystemEntryType> FsEntryTypes;

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

        if (!int.TryParse(input, CultureInfo.InvariantCulture, out var value))
        {
            index = default;
            return false;
        }

        if (!zeroBased && value == 0)
        {
            index = default;
            return false;
        }

        index = new Index(Math.Abs(value) - (zeroBased ? 0 : 1), value < 0);
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

    public static Index ParseIndex(ReadOnlySpan<char> input, bool zeroBased)
    {
        if (TryParseIndex(input, zeroBased, out var index))
        {
            return index;
        }

        throw new InvalidRequestException($"Invalid index value '{input.ToString()}'.");
    }

    public static async ValueTask<int?> ParseOffsetAsync(string? input, bool zeroBased, Func<ValueTask<int>> getCount)
    {
        if (input == null)
            return null;

        var index = ParseIndex(input, zeroBased);
        return index.IsFromEnd ? await getCount() - index.Value - 1 : index.Value;
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

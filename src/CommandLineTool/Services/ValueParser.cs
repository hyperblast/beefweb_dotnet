using System;
using System.Globalization;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class ValueParser
{
    private static readonly string[] FalseNames = ["false", "off"];
    private static readonly string[] TrueNames = ["true", "on"];
    private static readonly string[] ToggleNames = ["toggle"];
    private static readonly string[] DirectoryNames = ["d", "dir", "directory"];
    private static readonly string[] FileNames = ["f", "file"];

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
        if (MatchesName(value, FalseNames))
            return BoolSwitch.False;

        if (MatchesName(value, TrueNames))
            return BoolSwitch.True;

        if (MatchesName(value, ToggleNames))
            return BoolSwitch.Toggle;

        throw new InvalidRequestException($"Invalid bool value '{value}'.");
    }

    public static FileSystemEntryType ParseFileSystemEntryType(string value)
    {
        if (MatchesName(value, DirectoryNames))
            return FileSystemEntryType.Directory;

        if (MatchesName(value, FileNames))
            return FileSystemEntryType.File;

        throw new InvalidRequestException($"Invalid file system entry type '{value}'.");
    }

    private static bool MatchesName(string value, string[] names)
    {
        foreach (var name in names)
        {
            if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}

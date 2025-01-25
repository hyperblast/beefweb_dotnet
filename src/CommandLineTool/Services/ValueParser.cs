using System;
using System.Globalization;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class ValueParser
{
    private static readonly string[] FalseNames = ["false", "off"];
    private static readonly string[] TrueNames = ["true", "on"];
    private static readonly string[] ToggleNames = ["toggle"];

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
        return MatchesName(FalseNames, value, BoolSwitch.False) ??
               MatchesName(TrueNames, value, BoolSwitch.True) ??
               MatchesName(ToggleNames, value, BoolSwitch.Toggle) ??
               throw new InvalidRequestException($"Invalid bool value '{value}'.");
    }

    private static BoolSwitch? MatchesName(string[] names, string value, BoolSwitch result)
    {
        foreach (var name in names)
        {
            if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }
        }

        return null;
    }
}

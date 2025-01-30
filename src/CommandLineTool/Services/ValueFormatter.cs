using System;
using System.Globalization;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class ValueFormatter
{
    private static string CapitalizeFirstChar(this string str)
    {
        if (str.Length == 0)
        {
            return str;
        }

        return char.ToUpperInvariant(str[0]) + str[1..];
    }

    public static string FormatFileSize(this long value)
    {
        const long kb = 1024;
        const long mb = kb * 1024;
        const long gb = mb * 1024;

        return value switch
        {
            > gb => (value / gb).ToString(CultureInfo.InvariantCulture) + "G",
            > mb => (value / mb).ToString(CultureInfo.InvariantCulture) + "M",
            > kb => (value / kb).ToString(CultureInfo.InvariantCulture) + "K",
            _ => value.ToString(CultureInfo.InvariantCulture) + "b",
        };
    }

    public static string[] Format(this PlayerOption option, bool zeroBased)
    {
        return [option.Id.CapitalizeFirstChar(), option.FormatValue(includeNumericValue: true, zeroBased)];
    }

    public static string FormatValue(this PlayerOption option, bool includeNumericValue = false, bool zeroBased = false)
    {
        if (option.Type != PlayerOptionType.Enum)
        {
            return option.Value.ToString()!;
        }

        var index = (int)option.Value;
        var name = option.EnumNames![index];

        if (!includeNumericValue)
        {
            return name;
        }

        var displayIndex = index + (zeroBased ? 0 : 1);
        return $"{name} [{displayIndex}]";
    }

    public static string Format(this VolumeInfo volumeInfo)
    {
        var muteStatus = volumeInfo.IsMuted ? " [muted]" : "";
        return volumeInfo.Type switch
        {
            VolumeType.Db => volumeInfo.Value.ToString("0.0", CultureInfo.InvariantCulture) + " dB" + muteStatus,
            VolumeType.Linear => volumeInfo.Value.ToString("0") + muteStatus,
            _ => throw new ArgumentException($"Unknown volume type '{volumeInfo.Type}'."),
        };
    }

    public static string FormatAsTrackTime(this TimeSpan time)
    {
        return time.Ticks switch
        {
            > TimeSpan.TicksPerDay => time.ToString(@"d\.hh\:mm\:ss"),
            > TimeSpan.TicksPerHour => time.ToString(@"hh\:mm\:ss"),
            _ => time.ToString(@"mm\:ss")
        };
    }
}

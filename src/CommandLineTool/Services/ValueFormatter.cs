using System;
using System.Globalization;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class ValueFormatter
{
    public static string CapitalizeFirstChar(this string str)
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

    public static string[] Format(this PlayerOption option, bool zeroBasedIndexes)
    {
        return [option.Id.CapitalizeFirstChar(), option.FormatValue(zeroBasedIndexes)];
    }

    public static string FormatProgress(this ActiveItemInfo activeItem)
    {
        return activeItem.Position.FormatAsTrackTime() + " / " +
               activeItem.Duration.FormatAsTrackTime();
    }

    public static string FormatValue(this PlayerOption option, bool zeroBasedIndexes)
    {
        if (option.Type != PlayerOptionType.Enum)
        {
            return option.Value.ToString()!;
        }

        var index = (int)option.Value;
        var name = option.EnumNames![index];
        var displayIndex = index + (zeroBasedIndexes ? 0 : 1);
        return $"{name} [{displayIndex}]";
    }

    public static string Format(this VolumeInfo volumeInfo)
    {
        var muteStatus = volumeInfo.IsMuted ? " [muted]" : "";
        switch (volumeInfo.Type)
        {
            case VolumeType.Db:
                return volumeInfo.Value.ToString("0.0", CultureInfo.InvariantCulture) + " dB" + muteStatus;
            case VolumeType.Linear:
            case VolumeType.UpDown:
                return volumeInfo.Value.ToString("0") + muteStatus;
            default:
                throw new ArgumentException($"Unknown volume type '{volumeInfo.Type}'.");
        }
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

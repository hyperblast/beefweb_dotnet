using System;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class DisplayFormatter
{
    private static string CapitalizeFirstChar(this string str)
    {
        if (str.Length == 0)
        {
            return str;
        }

        return char.ToUpperInvariant(str[0]) + str[1..];
    }

    public static string[] Format(this PlayerOption option)
    {
        return [option.Id.CapitalizeFirstChar(), option.FormatValue()];
    }

    public static string FormatValue(this PlayerOption option)
    {
        return option.Value is int intValue ? option.EnumNames![intValue] : option.Value.ToString()!;
    }

    public static string Format(this VolumeInfo volumeInfo)
    {
        return volumeInfo.Type switch
        {
            VolumeType.Db => volumeInfo.Value.ToString("0.0") + " dB",
            VolumeType.Linear => volumeInfo.Value.ToString("0"),
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

    public static string FormatProgress(this ActiveItemInfo item)
    {
        return "[" + item.Position.FormatAsTrackTime() + " / " + item.Duration.FormatAsTrackTime() + "]";
    }
}

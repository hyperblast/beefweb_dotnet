using System;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class DisplayFormatter
{
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

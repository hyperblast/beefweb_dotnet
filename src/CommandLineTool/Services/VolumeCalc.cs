using System;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class VolumeCalc
{
    public static double AdjustDbPercentage(double db, double percentChange, VolumeInfo volumeInfo)
    {
        var percent = DbToPercent(db, volumeInfo);
        return PercentToDb(percent + percentChange, volumeInfo);
    }

    public static double PercentToDb(double percent, VolumeInfo volumeInfo)
    {
        return percent switch
        {
            >= 100 => volumeInfo.Max,
            <= 0 => volumeInfo.Min,
            _ => 10 * Math.Log2(percent / 100.0)
        };
    }

    public static double PercentToLinear(double percent, VolumeInfo volumeInfo)
    {
        return percent switch
        {
            >= 100 => volumeInfo.Max,
            <= 0 => volumeInfo.Min,
            _ => percent / 100 * (volumeInfo.Max - volumeInfo.Min) + volumeInfo.Min,
        };
    }

    public static double DbToPercent(double db, VolumeInfo volumeInfo)
    {
        db = Normalize(db, volumeInfo);
        return Math.Round(Math.Pow(2, db / 10) * 100);
    }

    public static double Normalize(double value, VolumeInfo volumeInfo)
    {
        return value > volumeInfo.Max
            ? volumeInfo.Max
            : value < volumeInfo.Min
                ? volumeInfo.Min
                : value;
    }
}

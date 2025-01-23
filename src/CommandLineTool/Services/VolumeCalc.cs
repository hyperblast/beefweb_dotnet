using System;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class VolumeCalc
{
    public static double LinearChange(double dbVolume, double linearChange, VolumeInfo volumeInfo)
    {
        var linear = DbToLinear(dbVolume, volumeInfo);
        return LinearToDb(linear + linearChange, volumeInfo);
    }

    public static double LinearToDb(double linearVolume, VolumeInfo volumeInfo)
    {
        return linearVolume switch
        {
            >= 100 => volumeInfo.Max,
            <= 0 => volumeInfo.Min,
            _ => 10 * Math.Log2(linearVolume / 100.0)
        };
    }

    public static double DbToLinear(double dbVolume, VolumeInfo volumeInfo)
    {
        dbVolume = NormalizeDb(dbVolume, volumeInfo);
        return Math.Round(Math.Pow(2, dbVolume / 10) * 100);
    }

    public static double NormalizeDb(double dbVolume, VolumeInfo volumeInfo)
    {
        return dbVolume > volumeInfo.Max
            ? volumeInfo.Max
            : dbVolume < volumeInfo.Min
                ? volumeInfo.Min
                : dbVolume;
    }
}

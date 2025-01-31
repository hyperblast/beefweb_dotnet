namespace Beefweb.CommandLineTool.Services;

public enum VolumeChangeType
{
    Linear = 0,
    Db = 1,
    Percent = 2,
}

public readonly record struct VolumeChange(VolumeChangeType Type, double Value);

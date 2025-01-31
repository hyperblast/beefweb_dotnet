using System;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("volume", "vol", Description = "Get or set volume",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class VolumeCommand(IClientProvider clientProvider, IConsole console) : ServerCommandBase(clientProvider)
{
    private const string DbSuffix = "db";

    [Option(T.Set, Description = "Set volume to absolute value (dB or linear in range [0..100])")]
    public string? AbsoluteValue { get; set; }

    [Option(T.Adjust, Description = "Adjust volume relative to current value (dB or linear in range [-100..100])")]
    public string? RelativeValue { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var volumeInfo = (await Client.GetPlayerState(null, ct)).Volume;

        if (AbsoluteValue == null && RelativeValue == null)
        {
            console.WriteLine(volumeInfo.Format());
            return;
        }

        var volumeChange = ValueParser.ParseVolumeChange(RelativeValue ?? AbsoluteValue);

        var newVolume = volumeInfo.Type switch
        {
            VolumeType.Db => CalculateVolumeDb(volumeInfo, volumeChange, RelativeValue != null),
            VolumeType.Linear => CalculateVolumeLinear(volumeInfo, volumeChange, RelativeValue != null),
            _ => throw new InvalidOperationException("Unknown volume type " + volumeInfo.Type),
        };

        await Client.SetVolume(newVolume, ct);
    }

    private static double CalculateVolumeLinear(VolumeInfo volumeInfo, VolumeChange volumeChange, bool isRelative)
    {
        return volumeChange.Type switch
        {
            VolumeChangeType.Db => throw new InvalidRequestException(
                "Current player does not support dB volume control."),

            VolumeChangeType.Linear => isRelative
                ? VolumeCalc.Normalize(volumeChange.Value + volumeInfo.Value, volumeInfo)
                : VolumeCalc.Normalize(volumeChange.Value, volumeInfo),

            VolumeChangeType.Percent => isRelative
                ? VolumeCalc.Normalize(
                    volumeInfo.Value + VolumeCalc.PercentToLinear(volumeChange.Value, volumeInfo),
                    volumeInfo)
                : VolumeCalc.PercentToLinear(volumeChange.Value, volumeInfo),

            _ => throw new InvalidOperationException("Unknown volume change type " + volumeChange.Type)
        };
    }

    private static double CalculateVolumeDb(VolumeInfo volumeInfo, VolumeChange volumeChange, bool isRelative)
    {
        return volumeChange.Type switch
        {
            VolumeChangeType.Linear or VolumeChangeType.Percent => isRelative
                ? VolumeCalc.AdjustDbPercentage(volumeInfo.Value, volumeChange.Value, volumeInfo)
                : VolumeCalc.PercentToDb(volumeChange.Value, volumeInfo),

            VolumeChangeType.Db => isRelative
                ? VolumeCalc.Normalize(volumeChange.Value + volumeInfo.Value, volumeInfo)
                : VolumeCalc.Normalize(volumeChange.Value, volumeInfo),

            _ => throw new InvalidOperationException("Unknown volume change type " + volumeChange.Type)
        };
    }
}

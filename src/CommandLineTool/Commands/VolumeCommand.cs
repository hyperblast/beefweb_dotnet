using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("volume", Description = "Get or set volume",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class VolumeCommand(IClientProvider clientProvider, IConsole console) : ServerCommandBase(clientProvider)
{
    private const string DbSuffix = "db";

    [Option(T.Set, Description = "Set volume to absolute value (dB or linear in range [0..100])")]
    public string? AbsoluteValue { get; set; }

    [Option("-a|--adjust", Description = "Adjust volume relative to current value (dB or linear in range [-100..100])")]
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

        var newVolumeString = (RelativeValue ?? AbsoluteValue)!;
        double newVolume;

        if (newVolumeString.EndsWith(DbSuffix, StringComparison.OrdinalIgnoreCase))
        {
            var value = ValueParser.ParseDouble(newVolumeString.AsSpan(..^DbSuffix.Length));

            newVolume = RelativeValue != null
                ? VolumeCalc.NormalizeDb(value + volumeInfo.Value, volumeInfo)
                : VolumeCalc.NormalizeDb(value, volumeInfo);
        }
        else
        {
            var value = ValueParser.ParseDouble(newVolumeString);

            newVolume = RelativeValue != null
                ? VolumeCalc.LinearChange(volumeInfo.Value, value, volumeInfo)
                : VolumeCalc.LinearToDb(value, volumeInfo);
        }

        await Client.SetVolume(newVolume, ct);
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("set-volume", Description = "Set volume")]
public class SetVolumeCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Argument(0, Description = "New volume (dB or linear in range [0..100])")]
    [Required]
    public string Volume { get; set; } = null!;

    [Option("-r|--relative", Description = "Adjust volume relative to current value")]
    public bool Relative { get; set; }

    [Option("-m|--minus", Description = "Interpret specified volume as negative")]
    public bool Minus { get; set; }

    private int MinusFactor => Minus ? -1 : 1;

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var volumeInfo = (await Client.GetPlayerState(null, ct)).Volume;
        double newVolume;

        if (Volume.EndsWith("db", StringComparison.OrdinalIgnoreCase))
        {
            var value = MinusFactor * ParseDouble(Volume.AsSpan(..^2));

            newVolume = Relative
                ? VolumeCalc.NormalizeDb(value + volumeInfo.Value, volumeInfo)
                : VolumeCalc.NormalizeDb(value, volumeInfo);
        }
        else
        {
            var value = MinusFactor * ParseDouble(Volume);

            newVolume = Relative
                ? VolumeCalc.LinearChange(volumeInfo.Value, value, volumeInfo)
                : VolumeCalc.LinearToDb(value, volumeInfo);
        }

        await Client.SetVolume(newVolume, ct);
    }

    private static double ParseDouble(ReadOnlySpan<char> valueString)
    {
        if (double.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new InvalidRequestException($"Invalid numeric value: {valueString}");
    }
}

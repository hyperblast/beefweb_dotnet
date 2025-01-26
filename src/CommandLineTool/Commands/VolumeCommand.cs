using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("volume", Description = "Get or set volume")]
public class VolumeCommand(IClientProvider clientProvider, IConsole console) : ServerCommandBase(clientProvider)
{
    private const string DbSuffix = "db";

    [Option(T.Value, Description = "New volume value (dB or linear in range [0..100])")]
    public string? Value { get; set; }

    [Option("-r|--relative", Description = "Adjust volume relative to current value")]
    public bool Relative { get; set; }

    [Option("-m|--minus", Description = "Interpret specified volume as negative")]
    public bool Minus { get; set; }

    private int MinusFactor => Minus ? -1 : 1;

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var volumeInfo = (await Client.GetPlayerState(null, ct)).Volume;

        if (Value == null)
        {
            console.WriteLine(volumeInfo.Format());
            return;
        }

        double newVolume;

        if (Value.EndsWith(DbSuffix, StringComparison.OrdinalIgnoreCase))
        {
            var value = MinusFactor * ParseDouble(Value.AsSpan(..^DbSuffix.Length));

            newVolume = Relative
                ? VolumeCalc.NormalizeDb(value + volumeInfo.Value, volumeInfo)
                : VolumeCalc.NormalizeDb(value, volumeInfo);
        }
        else
        {
            var value = MinusFactor * ParseDouble(Value);

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

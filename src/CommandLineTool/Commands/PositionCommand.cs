using System;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("position", "pos", "seek",
    Description = "Get or set playback position",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue,
    ExtendedHelpText =
        "\nPosition values could be specified in multiple formats:" +
        "\n  - Detailed time format [HH:]MM:SS" +
        "\n  - Number with time unit (e.g. 1m for 1 minute)" +
        "\n  - Percentage of current track length (e.g. 30%)" +
        "\n  - Plain number of seconds")]
public class PositionCommand(IClientProvider clientProvider, IConsole console) : ServerCommandBase(clientProvider)
{
    [Option(T.Set, Description = "Change position to absolute value (negative value means from end)")]
    public string? AbsoluteValue { get; set; }

    [Option(T.Adjust, Description = "Change position relative to current value")]
    public string? RelativeValue { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var state = await Client.GetPlayerState(null, ct);
        if (state.PlaybackState == PlaybackState.Stopped)
        {
            return;
        }

        var activeItem = state.ActiveItem;
        if (AbsoluteValue == null && RelativeValue == null)
        {
            console.WriteLine(activeItem.FormatProgress());
            return;
        }

        var position = ParsePosition((RelativeValue ?? AbsoluteValue)!, activeItem);

        if (RelativeValue != null)
        {
            await Client.SeekRelative(position, ct);
            return;
        }

        if (position >= TimeSpan.Zero)
        {
            await Client.SeekAbsolute(position, ct);
            return;
        }

        await Client.SeekAbsolute(position + activeItem.Duration, ct);
    }

    private static TimeSpan ParsePosition(string input, ActiveItemInfo activeItem)
    {
        if (!input.EndsWith('%'))
        {
            return PositionParser.Parse(input);
        }

        var positionPercent = ValueParser.ParseDouble(input.AsSpan()[..^1]);
        return positionPercent / 100 * activeItem.Duration;
    }
}

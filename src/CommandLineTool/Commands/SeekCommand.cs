using System;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("seek", Description = "Change playback position",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class SeekCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    public string[]? RemainingArguments { get; set; }

    [Option("-r|--relative", Description = "Change position relative to current value")]
    public bool Relative { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (RemainingArguments is not { Length: > 0 })
            throw new InvalidRequestException("New position parameter is required.");

        var newPositionStr = RemainingArguments[0];

        var state = await Client.GetPlayerState(null, ct);
        if (state.PlaybackState == PlaybackState.Stopped)
        {
            return;
        }

        TimeSpan newPosition;

        if (newPositionStr.EndsWith('%'))
        {
            var newPositionPercent = ValueParser.ParseDouble(newPositionStr.AsSpan()[..^1]);
            newPosition = newPositionPercent / 100 * state.ActiveItem.Duration;
        }
        else
        {
            newPosition = TimeSpan.FromSeconds(ValueParser.ParseDouble(newPositionStr));
        }

        if (Relative)
        {
            await Client.SeekRelative(newPosition, ct);
            return;
        }

        if (newPosition >= TimeSpan.Zero)
        {
            await Client.SeekAbsolute(newPosition, ct);
            return;
        }

        await Client.SeekAbsolute(newPosition + state.ActiveItem.Duration, ct);
    }
}

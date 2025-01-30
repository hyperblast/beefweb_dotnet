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

        var newPosition = RemainingArguments is { Length: > 0 }
            ? TimeSpan.FromSeconds(ValueParser.ParseDouble(RemainingArguments[0]))
            : throw new InvalidRequestException("New position parameter is required.");

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

        var state = await Client.GetPlayerState(null, ct);
        if (state.PlaybackState != PlaybackState.Stopped)
        {
            await Client.SeekAbsolute(newPosition + state.ActiveItem.Duration, ct);
        }
    }
}

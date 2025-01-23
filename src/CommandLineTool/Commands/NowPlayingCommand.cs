using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("now-playing", Description = "Display current track information")]
public class NowPlayingCommand(IClientProvider clientProvider, ITabularWriter writer) : ServerCommandBase(clientProvider)
{
    [Option(T.TrackColumns, Description = D.TrackColumnsCurrent)]
    public string[]? TrackColumns { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var formatExpressions = TrackColumns is { Length: > 0 } ? TrackColumns : ["%artist% - %title%"];
        var state = await Client.GetPlayerState(formatExpressions, ct);

        if (state.PlaybackState != PlaybackState.Stopped)
        {
            writer.WriteRow(state.ActiveItem.Columns.ToArray());
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("now-playing", "np", Description = "Display current track information")]
public class NowPlayingCommand(IClientProvider clientProvider, ITabularWriter writer, ISettingsStorage storage)
    : ServerCommandBase(clientProvider)
{
    [Option(T.ItemColumns, Description = D.CurrentItemColumns)]
    public string[]? ItemColumns { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var columns = ItemColumns.GetOrDefault(storage.Settings.NowPlayingFormat);
        var state = await Client.GetPlayerState(columns, ct);

        if (state.PlaybackState != PlaybackState.Stopped)
        {
            writer.WriteRow(state.ActiveItem.Columns.ToArray());
        }
    }
}

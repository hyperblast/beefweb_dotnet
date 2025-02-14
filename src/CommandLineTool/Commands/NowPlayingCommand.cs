using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("now-playing", "np", Description = "Display current track information")]
public class NowPlayingCommand(IClientProvider clientProvider, IConsole console, ISettingsStorage storage)
    : ServerCommandBase(clientProvider)
{
    [Option(T.Format, Description = D.CurrentItemFormat)]
    public string? Format { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var format = Format.GetOrDefault(storage.Settings.NowPlayingFormat);
        var state = await Client.GetPlayerState([format], ct);

        if (state.PlaybackState != PlaybackState.Stopped)
        {
            console.WriteLine(state.ActiveItem.Columns[0]);
        }
    }
}

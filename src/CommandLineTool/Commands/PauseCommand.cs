using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("pause", Description = "Pause or resume playback")]
public class PauseCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option("-p|--play-pause", Description = "Toggle paused state. Start playback if stopped")]
    public bool PlayPause { get; set; }

    [Option("-t|--toggle", Description = "Toggle paused state. Do nothing if stopped")]
    public bool Toggle { get; set; }

    [Option("-f|--off", Description = "Disable paused state. Do nothing if not paused")]
    public bool Off { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (PlayPause)
        {
            await Client.PlayOrPause(ct);
            return;
        }

        if (Toggle)
        {
            await Client.TogglePause(ct);
            return;
        }

        if (!Off)
        {
            await Client.Pause(ct);
            return;
        }

        var state = await Client.GetPlayerState(null, ct);
        if (state.PlaybackState == PlaybackState.Paused)
        {
            await Client.PlayCurrent(ct);
        }
    }
}

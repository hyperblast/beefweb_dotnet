using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("pause", Description = "Pause playback")]
public class PauseCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option("-t|--toggle", Description = "Toggle pause state instead of pausing")]
    public bool Toggle { get; set; }

    [Option("-f|--off", Description = "Disable paused state (does nothing if not paused)")]
    public bool Off { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (Toggle)
        {
            await Client.TogglePause(ct);
            return;
        }

        if (Off)
        {
            var state = await Client.GetPlayerState(null, ct);

            if (state.PlaybackState == PlaybackState.Paused)
            {
                await Client.PlayCurrent(ct);
            }

            return;
        }

        await Client.Pause(ct);
    }
}

using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("pause", Description = "Pause playback")]
public class PauseCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option("-t|--toggle", Description = "Toggle pause state instead of pausing")]
    public bool Toggle { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (Toggle)
        {
            await Client.TogglePause(ct);
        }
        else
        {
            await Client.Pause(ct);
        }
    }
}

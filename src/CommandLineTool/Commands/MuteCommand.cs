using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("mute", Description = "Mute audio output")]
public class MuteCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option("-t|--toggle", Description = "Toggle muted state instead of muting")]
    public bool Toggle { get; set; }

    [Option("-f|--off", Description = "Disable muted state (does nothing if not muted)")]
    public bool Off { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var change = Toggle
            ? BoolSwitch.Toggle
            : Off
                ? BoolSwitch.False
                : BoolSwitch.True;

        await Client.SetMuted(change, ct);
    }
}

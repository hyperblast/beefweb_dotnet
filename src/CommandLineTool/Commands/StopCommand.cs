using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("stop", Description = "Stop playback")]
public class StopCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    private readonly IClientProvider _clientProvider = clientProvider;

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);
        await _clientProvider.Client.Stop(ct);
    }
}

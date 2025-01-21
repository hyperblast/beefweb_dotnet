using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("play", Description = "Play current track")]
public class PlayCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        await Client.PlayCurrent(ct);
    }
}

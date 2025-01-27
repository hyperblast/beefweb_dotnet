using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

public abstract class ServerCommandBase(IClientProvider clientProvider) : CommandBase
{
    [Option("-S|--server", Description = "Server to use. Could be a server URL or name of the predefined server.")]
    public string? Server { get; set; }

    protected IPlayerClient Client => clientProvider.Client;

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(Server))
        {
            clientProvider.ServerName = Server;
        }

        return Task.CompletedTask;
    }
}

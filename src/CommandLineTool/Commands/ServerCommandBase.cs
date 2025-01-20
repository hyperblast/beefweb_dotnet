using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

public class ServerCommandBase(IClientProvider clientProvider) : CommandBase
{
    [Option("-s|--server", Description = "Server to use. Could be a server URL or name of the predefined server.")]
    public string? Server { get; set; }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(Server))
        {
            clientProvider.ServerName = Server;
        }

        return Task.CompletedTask;
    }
}

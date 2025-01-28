using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

public abstract class ServerCommandBase(IClientProvider clientProvider) : CommandBase
{
    [Option(T.Server, Description = D.Server)]
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

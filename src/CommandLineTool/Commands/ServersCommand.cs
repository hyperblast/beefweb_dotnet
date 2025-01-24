using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("servers", Description = "List or modify predefined servers")]
[Subcommand(typeof(ServersAddCommand))]
[Subcommand(typeof(ServersDeleteCommand))]
[Subcommand(typeof(ServersSetDefaultCommand))]
public class ServersCommand(ISettingsStorage storage, ITabularWriter writer) : CommandBase
{
    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        var rows = settings.PredefinedServers
            .OrderBy(i => i.Key)
            .Select(kv => new[] { kv.Key, kv.Value.ToString(), settings.IsDefaultServer(kv.Key) ? "(default)" : "" })
            .ToList();

        writer.WriteTable(rows);

        return Task.CompletedTask;
    }
}

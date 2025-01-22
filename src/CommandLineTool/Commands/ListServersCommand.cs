using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("list-servers", Description = "List predefined servers")]
public class ListServersCommand(ISettingsStorage storage, ITabularWriter writer) : CommandBase
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

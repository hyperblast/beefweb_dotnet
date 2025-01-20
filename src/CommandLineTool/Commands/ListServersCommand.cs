using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("list-servers", Description = "List predefined servers")]
public class ListServersCommand(ISettingsStorage storage, IConsole console) : CommandBase
{
    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        foreach (var (name, uri) in settings.PredefinedServers.OrderBy(i => i.Key))
        {
            var isDefault = string.Equals(settings.DefaultServer, name, StringComparison.OrdinalIgnoreCase);

            console.Write(name);
            console.Write("\t\t");
            console.Write(uri);

            if (isDefault)
            {
                console.Write("\t\t");
                console.Write("(default)");
            }

            console.WriteLine();
        }

        return Task.CompletedTask;
    }
}

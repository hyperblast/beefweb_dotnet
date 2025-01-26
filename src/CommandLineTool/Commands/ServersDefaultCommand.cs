using System;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("default", Description = "Get or set default server")]
public class ServersDefaultCommand(ISettingsStorage storage, IConsole console) : CommandBase
{
    private const string NoneServer = "none";

    [Option(T.Value, Description = "New default server name or 'none' to unset default server")]
    public string? Server { get; set; } = null!;

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        if (Server == null)
        {
            if (settings.DefaultServer != null &&
                settings.PredefinedServers.TryGetValue(settings.DefaultServer, out var url))
            {
                console.WriteLine(url + " [" + settings.DefaultServer + "]");
            }
            else
            {
                console.WriteLine(NoneServer);
            }

            return Task.CompletedTask;
        }

        if (string.Equals(Server, NoneServer, StringComparison.OrdinalIgnoreCase))
        {
            settings.DefaultServer = null;
        }
        else
        {
            if (!settings.PredefinedServers.ContainsKey(Server))
            {
                throw new InvalidRequestException($"Unknown server '{Server}'.");
            }

            settings.DefaultServer = Server;
        }

        storage.Save();
        return Task.CompletedTask;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("delete-server", Description = "Delete predefined server")]
public class DeleteServerCommand(ISettingsStorage storage) : CommandBase
{
    [Argument(0, Description = "Server name")]
    [Required]
    public string Name { get; set; } = null!;

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        settings.PredefinedServers.Remove(Name);

        if (string.Equals(settings.DefaultServer, Name, StringComparison.OrdinalIgnoreCase))
        {
            settings.DefaultServer = settings.PredefinedServers.Keys.Order().FirstOrDefault();
        }

        storage.Save();
        return Task.CompletedTask;
    }
}

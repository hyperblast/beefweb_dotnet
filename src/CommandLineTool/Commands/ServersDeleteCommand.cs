using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("delete", Description = "Delete predefined server")]
public class ServersDeleteCommand(ISettingsStorage storage) : CommandBase
{
    [Argument(0, Description = "Server name")]
    [Required]
    public string Name { get; set; } = null!;

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        settings.PredefinedServers.Remove(Name);

        if (settings.IsDefaultServer(Name))
        {
            settings.DefaultServer = null;
        }

        storage.Save();
        return Task.CompletedTask;
    }
}

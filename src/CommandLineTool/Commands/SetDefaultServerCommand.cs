using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("set-default", Description = "Set default server")]
public class SetDefaultServerCommand(ISettingsStorage storage) : CommandBase
{
    [Argument(0)]
    [Required]
    public string Name { get; set; } = null!;

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        if (!settings.PredefinedServers.ContainsKey(Name))
        {
            throw new InvalidRequestException($"Unknown server '{Name}'.");
        }

        settings.DefaultServer = Name;
        storage.Save();
        return Task.CompletedTask;
    }
}

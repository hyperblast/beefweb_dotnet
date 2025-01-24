using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("set-default", Description = "Set default server")]
public class ServersSetDefaultCommand(ISettingsStorage storage) : CommandBase
{
    [Argument(0, Description = "Server name or 'none' to unset default server")]
    [Required]
    public string Name { get; set; } = null!;

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        if (!string.Equals(Name, "none", StringComparison.OrdinalIgnoreCase))
        {
            if (!settings.PredefinedServers.ContainsKey(Name))
            {
                throw new InvalidRequestException($"Unknown server '{Name}'.");
            }

            settings.DefaultServer = Name;
        }
        else
        {
            settings.DefaultServer = null;
        }

        storage.Save();
        return Task.CompletedTask;
    }
}

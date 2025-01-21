using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("add-server", Description = "Add predefined server")]
public class AddServerCommand(ISettingsStorage storage) : CommandBase
{
    [Argument(0, Description = "Server name")]
    [Required]
    public string Name { get; set; } = null!;

    [Argument(1, Name = "Url", Description = "Server URL")]
    [Required]
    [HttpUri]
    public Uri Uri { get; set; } = null!;

    [Option("-f|--force", Description = "Force overwriting existing entry")]
    public bool Force { get; set; }

    [Option("--set-default", Description = "Set added server as default")]
    public bool SetDefault { get; set; }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        var settings = storage.Settings;

        if (!Force && settings.PredefinedServers.ContainsKey(Name))
        {
            throw new InvalidRequestException($"Server with name '{Name}' is already defined.");
        }

        settings.PredefinedServers[Name] = Uri;

        if (SetDefault)
        {
            settings.DefaultServer = Name;
        }

        storage.Save();
        return Task.CompletedTask;
    }
}

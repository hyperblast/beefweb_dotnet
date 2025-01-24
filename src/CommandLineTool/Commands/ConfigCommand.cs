using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("config", Description = "Get or set configuration option")]
public class ConfigCommand(ITabularWriter writer, ISettingsAccessor accessor, ISettingsStorage storage) : CommandBase
{
    [Argument(0, Description = "Option name")]
    [Required]
    public string Name { get; set; } = null!;

    [Option("-v|--value", Description = "New option value")]
    public string[]? Values { get; set; }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        if (Values is { Length: > 0 })
        {
            accessor.SetValues(Name, Values);
            storage.Save();
            return Task.CompletedTask;
        }

        writer.WriteRow(accessor.GetValues(Name));
        return Task.CompletedTask;
    }
}

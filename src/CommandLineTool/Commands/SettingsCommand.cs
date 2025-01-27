using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("settings", Description = "List client settings, get or set client setting")]
public class SettingsCommand(ITabularWriter writer, ISettingsAccessor accessor, ISettingsStorage storage) : CommandBase
{
    [Argument(0, Description = "Setting name")]
    public string? Name { get; set; } = null!;

    [Option(T.Set, Description = "New setting value")]
    public string[]? Values { get; set; }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        if (Name == null)
        {
            var allSettings = accessor.GetAllValues()
                .OrderBy(a => a.Key)
                .Select(a => (string[]) [a.Key, ..a.Value])
                .ToList();

            writer.WriteTable(allSettings);
            return Task.CompletedTask;
        }

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

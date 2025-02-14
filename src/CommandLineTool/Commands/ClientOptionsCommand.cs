using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("client-options", "cloptions", Description = "List client options, get or set client option")]
public class ClientOptionsCommand(ITabularWriter writer, ISettingsAccessor accessor, ISettingsStorage storage) : CommandBase
{
    [Argument(0, Description = "Option name")]
    public string? Name { get; set; } = null!;

    [Option(T.Set, Description = "New option value (could be specified multiple times)")]
    public string[]? Values { get; set; }

    [Option("-r|--reset", Description = "Reset option value to default")]
    public bool Reset { get; set; }

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

        if (Reset)
        {
            accessor.ResetValues(Name);
            storage.Save();
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

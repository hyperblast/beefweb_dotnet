using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("client-options", "clopt", Description = "List client options, get or set client option")]
public class ClientOptionsCommand(
    ITabularWriter writer,
    ISettingsAccessor accessor,
    ISettingsStorage storage,
    IConsole console)
    : CommandBase
{
    [Argument(0, Description = "Option name")]
    public string? Name { get; set; } = null!;

    [Option(T.Set, Description = "New option value (could be specified multiple times for multi-value settings)")]
    public string[]? Values { get; set; }

    [Option("-r|--reset", Description = "Reset option value to default")]
    public bool Reset { get; set; }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        if (Name == null)
        {
            var allValues = accessor.GetAllValues().ToList();

            console.WriteLine("Single value:");
            WriteOptions(allValues.Where(v => !v.isMultiValue));
            console.WriteLine();
            console.WriteLine("Multiple values:");
            WriteOptions(allValues.Where(v => v.isMultiValue));

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

        writer.WriteRow(accessor.GetValues(Name).Select(Quote).ToList());
        return Task.CompletedTask;
    }

    private void WriteOptions(IEnumerable<(string name, bool isMultiValue, List<string> values)> allValues)
    {
        var rows = allValues
            .OrderBy(a => a.name)
            .Select(a => (string[]) ["  " + a.name, ..a.values.Select(Quote)])
            .ToList();

        writer.WriteTable(rows);
    }

    private static string Quote(string s)
    {
        return s.Length > 0 && char.IsWhiteSpace(s[0]) || char.IsWhiteSpace(s[^1]) ? $"\"{s}\"" : s;
    }
}

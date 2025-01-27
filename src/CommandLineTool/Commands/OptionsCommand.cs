using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("options", Description = "List player options, get or set player option")]
public class OptionsCommand(IClientProvider clientProvider, IConsole console, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    [Argument(0, Description = "Option name")]
    public string? Name { get; set; } = null!;

    [Option(T.Set, Description = "New option value")]
    public string? Value { get; set; }

    [Option("-t|--short", Description = "Display only current enum value (not all possible values)")]
    public bool Short { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var state = await Client.GetPlayerState(null, ct);

        if (Name == null)
        {
            writer.WriteTable(state.Options.Select(o => o.Format()).ToList());
            return;
        }

        var option = GetOption(state.Options);

        if (Value != null)
        {
            await Client.SetOption(GetChangeRequest(option), ct);
            return;
        }

        if (Short || option.Type != PlayerOptionType.Enum)
        {
            console.WriteLine(option.FormatValue());
            return;
        }

        var currentValue = (int)option.Value;
        var i = 0;

        foreach (var name in option.EnumNames!)
        {
            console.Write(currentValue == i ? "* " : "  ");
            console.WriteLine(name);
            i++;
        }
    }

    private SetOptionRequest GetChangeRequest(PlayerOption option)
    {
        return option.Type switch
        {
            PlayerOptionType.Enum => new SetOptionRequest(option.Id, ValueParser.ParseEnumName(option, Value!)),
            PlayerOptionType.Bool => new SetOptionRequest(option.Id, ValueParser.ParseSwitch(Value!)),
            _ => throw new ArgumentException($"Unknown option type '{option.Type}'.")
        };
    }

    private PlayerOption GetOption(IList<PlayerOption> options)
    {
        foreach (var option in options)
        {
            if (string.Equals(option.Id, Name, StringComparison.OrdinalIgnoreCase))
            {
                return option;
            }
        }

        throw new InvalidRequestException($"Unknown option name '{Name}'.");
    }
}

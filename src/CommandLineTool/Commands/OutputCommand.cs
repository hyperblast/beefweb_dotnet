using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("outputs", "out", Description = "Show information about output devices or change output device")]
public class OutputCommand(IClientProvider clientProvider, IConsole console, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    [Option("-a|--all", Description = "Print all available output devices")]
    public bool All { get; set; }

    [Option("-t|--set-type", Description = "Set new output type")]
    public string? TypeId { get; set; }

    [Option("-d|--set-device", Description = "Set new output device")]
    public string? DeviceId { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var outputs = await Client.GetOutputs(ct);

        if (DeviceId != null)
        {
            await SetOutputDevice(outputs, ct);
            return;
        }

        if (TypeId != null)
        {
            throw new InvalidRequestException("Invalid option combination: --set-type requires --set-device.");
        }

        if (All)
            PrintAll(outputs);
        else
            PrintCurrent(outputs);
    }

    private void PrintAll(OutputsInfo outputs)
    {
        foreach (var type in outputs.Types)
        {
            var isCurrentType = type.Id == outputs.Active.TypeId;
            var rows = Enumerable.Empty<string[]>();

            if (outputs.SupportsMultipleOutputTypes)
            {
                rows = rows.Concat([
                    [" ", type.Id, type.Name],
                    [" ", "---", "---"]
                ]);
            }

            rows = rows.Concat(type.Devices.Select(d => (string[]) [
                isCurrentType && d.Id == outputs.Active.DeviceId ? "*" : " ",
                d.Id,
                d.FormatDeviceName()
            ]));

            writer.WriteTable(rows.ToList());
            console.WriteLine();
        }
    }

    private void PrintCurrent(OutputsInfo outputs)
    {
        var currentType = outputs.Types.First(c => c.Id == outputs.Active.TypeId);
        var currentDevice = currentType.Devices.First(c => c.Id == outputs.Active.DeviceId);

        var rows = Enumerable.Empty<string[]>();

        if (outputs.SupportsMultipleOutputTypes)
            rows = rows.Append([currentType.Id, currentType.Name]);

        rows = rows.Append([currentDevice.Id, currentDevice.FormatDeviceName()]);

        writer.WriteTable(rows.ToList());
    }

    private async ValueTask SetOutputDevice(OutputsInfo outputs, CancellationToken ct)
    {
        OutputTypeInfo type;

        if (TypeId != null)
        {
            type = outputs.Types.FirstOrDefault(c => string.Equals(c.Id, TypeId, StringComparison.OrdinalIgnoreCase))
                   ?? throw new InvalidRequestException($"Unknown output type: {TypeId}.");
        }
        else
        {
            type = outputs.Types.First(c => c.Id == outputs.Active.TypeId);
        }

        var device = type.Devices.FirstOrDefault(c => string.Equals(c.Id, DeviceId, StringComparison.OrdinalIgnoreCase))
                     ?? throw new InvalidRequestException($"Unknown output device: ${DeviceId}.");

        await Client.SetOutputDevice(type.Id, device.Id, ct);
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("add", Description = "Add playlist items",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class AddCommand(IClientProvider clientProvider, IConsole console) : ServerCommandBase(clientProvider)
{
    [Option(T.Playlist, CommandOptionType.SingleValue, Description = D.PlaylistToAddTo)]
    public PlaylistRef Playlist { get; set; } = PlaylistRef.Current;

    [Option(T.Position, Description = D.PositionForItems)]
    public int? Position { get; set; }

    [Option("-w|--no-wait", Description = "Do not wait for adding to complete")]
    public bool NoWait { get; set; }

    [Option("-g|--play", Description = "Start playing added items")]
    public bool Play { get; set; }

    [Option("-r|--replace", Description = "Replace existing items in the playlist")]
    public bool Replace { get; set; }

    [Option("-i|--stdin", Description = "Read items from standard input")]
    public bool ReadFromStdin { get; set; }

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var items = new List<string>();

        if (RemainingArguments != null)
        {
            items.AddRange(items);
        }

        if (ReadFromStdin)
        {
            var line = await console.In.ReadLineAsync(ct);
            while (line != null)
            {
                line = line.Trim();

                if (line.Length > 0 && line[0] != '#')
                {
                    items.Add(line);
                }

                line = await console.In.ReadLineAsync(ct);
            }
        }

        if (items.Count == 0)
        {
            throw new InvalidRequestException("At least one item is required.");
        }

        await Client.AddPlaylistItems(Playlist, items, new AddPlaylistItemsOptions
        {
            TargetPosition = Replace ? null : Position,
            ProcessAsynchronously = NoWait,
            ReplaceExistingItems = Replace,
            PlayAddedItems = Play,
        }, ct);
    }
}

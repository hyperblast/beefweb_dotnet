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

    [Option(T.Stdin, Description = D.StdinItems)]
    public bool ReadFromStdin { get; set; }

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var items = new List<string>();

        if (RemainingArguments != null)
        {
            items.AddRange(RemainingArguments);
        }

        if (ReadFromStdin)
        {
            await foreach (var line in console.In.ReadLinesAsync().WithCancellation(ct))
            {
                var s = line.Trim();
                if (s.Length > 0 && s[0] != '#')
                {
                    items.Add(s);
                }
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

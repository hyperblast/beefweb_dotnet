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

[Command("delete", Description = "Delete playlist items",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class DeleteCommand(IClientProvider clientProvider, IConsole console) : ServerCommandBase(clientProvider)
{
    [Option("-a|--all", Description = "Delete all playlist items (clear playlist)")]
    public bool All { get; set; }

    [Option(T.Playlist, CommandOptionType.SingleValue, Description = D.PlaylistToUse)]
    public PlaylistRef Playlist { get; set; } = PlaylistRef.Current;

    [Option(T.Stdin, Description = D.StdinIndices)]
    public bool ReadFromStdin { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (All)
        {
            await Client.ClearPlaylist(Playlist, ct);
            return;
        }

        var ranges = new List<Range>();

        if (RemainingArguments != null)
        {
            ranges.AddRange(RemainingArguments.Select(a => ValueParser.ParseRange(a, IndicesFrom0)));
        }

        if (ReadFromStdin)
        {
            await foreach (var token in console.In.ReadTokensAsync().WithCancellation(ct))
            {
                ranges.Add(ValueParser.ParseRange(token, IndicesFrom0));
            }
        }

        if (ranges.Count == 0)
        {
            throw new InvalidRequestException("At least one item is required.");
        }

        var indices = new HashSet<int>();
        var totalCount = await Client.GetItemCount(Playlist, ct);

        foreach (var range in ranges)
        {
            indices.AddRange(range.GetItems(totalCount));
        }

        if (indices.Count > 0)
        {
            await Client.RemovePlaylistItems(Playlist, indices.ToArray(), ct);
        }
    }
}

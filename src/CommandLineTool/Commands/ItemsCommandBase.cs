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

public abstract class ItemsCommandBase(IClientProvider clientProvider, IConsole console)
    : ServerCommandBase(clientProvider)
{
    [Option("-a|--all", Description = "Process all items in the playlist")]
    public bool All { get; set; }

    [Option(T.Playlist, Description = D.PlaylistToUse)]
    public string Playlist { get; set; } = Constants.CurrentPlaylist;

    [Option(T.Stdin, Description = D.StdinItemRanges)]
    public bool ReadFromStdin { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public string[]? RemainingArguments { get; set; }

    protected IList<PlaylistInfo> AllPlaylists { get; private set; } = Array.Empty<PlaylistInfo>();

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        AllPlaylists = await Client.GetPlaylists(ct);
        var playlist = AllPlaylists.Get(Playlist, IndicesFrom0);

        if (All)
        {
            await ProcessAllAsync(playlist, ct);
            return;
        }

        var ranges = new List<Range>();

        if (RemainingArguments != null)
        {
            ranges.AddRange(RemainingArguments.Select(a => RangeParser.Parse(a, IndicesFrom0)));
        }

        if (ReadFromStdin)
        {
            await foreach (var token in console.In.ReadTokensAsync().WithCancellation(ct))
            {
                ranges.Add(RangeParser.Parse(token, IndicesFrom0));
            }
        }

        if (ranges.Count == 0)
        {
            throw new InvalidRequestException("At least one item index or range is required.");
        }

        var items = new HashSet<int>();

        foreach (var range in ranges)
        {
            items.AddRange(range.GetItems(playlist.ItemCount));
        }

        if (items.Count > 0)
        {
            await ProcessAsync(playlist, items.ToArray(), ct);
        }
    }

    protected abstract Task ProcessAsync(PlaylistInfo playlist, IReadOnlyList<int> items, CancellationToken ct);

    protected abstract Task ProcessAllAsync(PlaylistInfo playlist, CancellationToken ct);
}

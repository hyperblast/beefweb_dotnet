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

    [Option(T.ItemIndex, Description = D.StartingItemIndex)]
    public int? ItemIndex { get; set; }

    [Option(T.Count, Description = D.DeleteCount)]
    public int? Count { get; set; }

    [Option(T.Stdin, Description = D.StdinIndices)]
    public bool ReadFromStdin { get; set; }

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (All)
        {
            await Client.ClearPlaylist(Playlist, ct);
            return;
        }

        var indices = new HashSet<int>();

        if (ItemIndex != null)
        {
            var index = ItemIndex.Value;
            var count = Count ?? (await GetPlaylist(ct)).ItemCount - index;
            indices.AddRange(Enumerable.Range(index, count));
        }

        if (RemainingArguments != null)
        {
            indices.AddRange(RemainingArguments.Select(a => ValueParser.ParseIndex(a)));
        }

        if (ReadFromStdin)
        {
            await foreach (var token in console.In.ReadTokensAsync().WithCancellation(ct))
            {
                indices.Add(ValueParser.ParseIndex(token));
            }
        }

        if (indices.Count == 0)
        {
            throw new InvalidRequestException("At least one item is required.");
        }

        await Client.RemovePlaylistItems(Playlist, indices.ToArray(), ct);
    }

    private async ValueTask<PlaylistInfo> GetPlaylist(CancellationToken ct)
    {
        // TODO: use get single playlist API

        var playlists = await Client.GetPlaylists(ct);
        if (Playlist == PlaylistRef.Current)
        {
            return playlists.SingleOrDefault(p => p.IsCurrent) ?? playlists.First();
        }

        return Playlist.Id != null ? playlists.Single(p => p.Id == Playlist.Id) : playlists[Playlist.Index];
    }
}

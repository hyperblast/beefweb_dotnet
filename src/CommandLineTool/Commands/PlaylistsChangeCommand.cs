using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("change", Description = "Change playlist",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class PlaylistsChangeCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option(T.Title, Description = D.PlaylistTitle)]
    public string? Title { get; set; }

    [Option(T.Position, Description = D.PositionForPlaylist)]
    public string? Position { get; set; }

    [Option(T.Select, Description = D.Select)]
    public bool Select { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (RemainingArguments is not { Length: > 0 })
        {
            throw new InvalidRequestException("Playlist parameter is required.");
        }

        var playlists = await Client.GetPlaylists(ct);
        var playlist = playlists.Get(RemainingArguments[0], IndicesFrom0);

        var newPosition = Position != null
            ? ValueParser.ParseOffset(Position, IndicesFrom0, playlists.Count)
            : (int?)null;

        if (Title != null)
        {
            await Client.SetPlaylistTitle(playlist.Id, Title, ct);
        }

        if (newPosition != null)
        {
            await Client.MovePlaylist(playlist.Id, newPosition.Value, ct);
        }

        if (Select)
        {
            await Client.SetCurrentPlaylist(playlist.Id, ct);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("add", Description = "Add to playback queue",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class QueueAddCommand(IClientProvider clientProvider)
    : ServerCommandBase(clientProvider)
{
    [Option(T.Playlist, Description = D.PlaylistToUse)]
    public string Playlist { get; set; } = Constants.CurrentPlaylist;

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var playlists = await Client.GetPlaylists(ct);
        var playlist = playlists.Get(Playlist, IndicesFrom0);

        var itemIndex = RemainingArguments is { Length: > 0 }
            ? IndexParser.ParseAndGetOffset(RemainingArguments[0], IndicesFrom0, playlist.ItemCount)
            : throw new InvalidRequestException("Item index parameter is required.");

        await Client.AddToPlayQueue(playlist.Id, itemIndex, cancellationToken: ct);
    }
}

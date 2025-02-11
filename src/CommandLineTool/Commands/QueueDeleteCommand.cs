using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("delete", "del", "rm", Description = "Delete from playback queue")]
public class QueueDeleteCommand(IClientProvider clientProvider)
    : ServerCommandBase(clientProvider)
{
    [Option(T.Playlist, Description = D.PlaylistToUse)]
    public string Playlist { get; set; } = Constants.CurrentPlaylist;

    [Option(T.TrackIndex, Description = D.DeleteTrackAtQueueIndex)]
    public string? ItemIndex { get; set; }

    [Option(T.Position, Description = D.DeleteTrackAtPlaylistIndex)]
    public string? QueueIndex { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (QueueIndex != null)
        {
            var queue = await Client.GetPlayQueue(cancellationToken: ct);
            var queueIndex = IndexParser.ParseAndGetOffset(QueueIndex, IndicesFrom0, queue.Count);
            await Client.RemoveFromPlayQueue(queueIndex, ct);
            return;
        }

        if (ItemIndex != null)
        {
            var playlists = await Client.GetPlaylists(ct);
            var playlist = playlists.Get(Playlist, IndicesFrom0);
            var itemIndex = IndexParser.ParseAndGetOffset(ItemIndex, IndicesFrom0, playlist.ItemCount);
            await Client.RemoveFromPlayQueue(playlist.Id, itemIndex, cancellationToken: ct);
            return;
        }

        throw new InvalidRequestException("Either --track or --position is required.");
    }
}

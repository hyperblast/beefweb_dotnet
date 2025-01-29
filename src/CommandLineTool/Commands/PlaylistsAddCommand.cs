using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("add", Description = "Add playlist")]
public class PlaylistsAddCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option("-t|--title", Description = "Playlist title")]
    public string? Title { get; set; }

    [Option(T.Position, Description = D.PositionForPlaylist)]
    public string? Position { get; set; }

    [Option("-c|--set-current", Description = "Select created playlist")]
    public bool SetCurrent { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var position = await ValueParser.ParseOffsetAsync(Position, IndicesFrom0, () => Client.GetPlaylistCount(ct));

        await Client.AddPlaylist(Title, position, ct);

        if (SetCurrent)
        {
            // TODO: use Id from response

            var playlists = await Client.GetPlaylists(ct);
            var playlistId = position != null ? playlists[position.Value].Id : playlists.Last().Id;
            await Client.SetCurrentPlaylist(playlistId, ct);
        }
    }
}

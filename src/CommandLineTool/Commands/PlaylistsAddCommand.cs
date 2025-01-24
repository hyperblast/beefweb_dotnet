using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("add", Description = "Add playlist")]
public class PlaylistsAddCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option("-t|--title", Description = "Playlist title")]
    public string? Title { get; set; }

    [Option("-p|--position", Description = "Position to insert playlist at")]
    public int? Position { get; set; }

    [Option("-c|--set-current", Description = "Select created playlist")]
    public bool SetCurrent { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        await Client.AddPlaylist(Title, Position, ct);

        if (SetCurrent)
        {
            var playlists = await Client.GetPlaylists(ct);
            var playlistId = Position >= 0 ? playlists[Position.Value].Id : playlists.Last().Id;

            await Client.SetCurrentPlaylist(playlistId, ct);
        }
    }
}

using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

public abstract class TransferItemsCommandBase(IClientProvider clientProvider, IConsole console)
    : ItemsCommandBase(clientProvider, console)
{
    [Option(CommonOptions.T.TargetPlaylist, Description = CommonOptions.D.TargetPlaylist)]
    public string? TargetPlaylist { get; set; }

    [Option(CommonOptions.T.Position, Description = CommonOptions.D.PositionForItems)]
    public string? Position { get; set; }

    protected (PlaylistInfo playlist, int? index) GetTargetPlaylistAndIndex(PlaylistInfo sourcePlaylist)
    {
        var playlist = TargetPlaylist != null
            ? AllPlaylists.Get(TargetPlaylist, IndicesFrom0)
            : sourcePlaylist;

        var index = Position != null
            ? IndexParser.ParseAndGetOffset(Position, IndicesFrom0, playlist.ItemCount)
            : (int?)null;

        return (playlist, index);
    }
}

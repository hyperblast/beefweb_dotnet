using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("add", Description = "Add playlist")]
public class PlaylistsAddCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option(T.Title, Description = D.PlaylistTitle)]
    public string? Title { get; set; }

    [Option(T.Position, Description = D.PositionForPlaylist)]
    public string? Position { get; set; }

    [Option(T.Select, Description = D.Select)]
    public bool Select { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var position = Position != null
            ? IndexParser.ParseAndGetOffset(Position, IndicesFrom0, await Client.GetPlaylistCount(ct))
            : (int?)null;

        await Client.AddPlaylist(Title, position, Select, ct);
    }
}

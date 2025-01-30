using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("move", "move", Description = "Move playlist to different position",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class PlaylistsMoveCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    public string[]? RemainingArguments { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (RemainingArguments is not { Length: >= 2 })
        {
            throw new InvalidRequestException("Playlist and new position parameters are required.");
        }

        var playlists = await Client.GetPlaylists(ct);
        var playlist = playlists.Get(RemainingArguments[0], IndicesFrom0);
        var newPosition = ValueParser.ParseOffset(RemainingArguments[1], IndicesFrom0, playlists.Count);

        await Client.MovePlaylist(playlist.Id, newPosition, ct);
    }
}

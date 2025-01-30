using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("delete", "del", "rm", Description = "Delete playlist",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class PlaylistsDeleteCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    public string[]? RemainingArguments { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var playlistRef = RemainingArguments is { Length: > 0 }
            ? RemainingArguments[0]
            : throw new InvalidRequestException("Playlist parameter is required.");

        var playlist = await Client.GetPlaylist(playlistRef, IndicesFrom0, ct);

        await Client.RemovePlaylist(playlist.Id, ct);
    }
}

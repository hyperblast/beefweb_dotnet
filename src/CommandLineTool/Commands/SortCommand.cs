using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("sort", Description = "Sort playlist items")]
public class SortCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option(T.Playlist, Description = D.PlaylistToUse)]
    public string Playlist { get; set; } = Constants.CurrentPlaylist;

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    [Option("-d|--desc", Description = "Sort in descending order")]
    public bool Descending { get; set; }

    [Option("-r|--random", Description = "Sort randomly")]
    public bool Random { get; set; }

    [Argument(0, Description = "Formatting expression to sort by")]
    public string? Expression { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var playlist = await Client.GetPlaylist(Playlist, IndicesFrom0, ct);

        if (Random)
        {
            await Client.SortPlaylistRandomly(playlist.Id, ct);
            return;
        }

        if (Expression == null)
        {
            throw new InvalidRequestException("Expression to sort by or --random is required.");
        }

        await Client.SortPlaylist(playlist.Id, Expression, Descending, ct);
    }
}

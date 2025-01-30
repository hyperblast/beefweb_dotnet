using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("play", Description = "Play current or specified track")]
public class PlayCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Option(T.Playlist, Description = D.PlaylistToUse)]
    public string Playlist { get; set; } = Constants.CurrentPlaylist;

    [Option(T.ItemIndex, Description = D.ItemIndex)]
    public string? ItemIndex { get; set; }

    [Option("-n|--next", Description = "Play next item")]
    public bool Next { get; set; }

    [Option("--next-by", Description = "Play next item by specified formatting expression (e.g. %album%)")]
    public string? NextBy { get; set; }

    [Option("-r|--previous", Description = "Play previous item")]
    public bool Previous { get; set; }

    [Option("--previous-by", Description = "Play previous item by specified formatting expression (e.g. %album%)")]
    public string? PreviousBy { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (ItemIndex != null)
        {
            var playlist = await Client.GetPlaylist(Playlist, IndicesFrom0, ct);
            var position = ValueParser.ParseOffset(ItemIndex, IndicesFrom0, playlist.ItemCount);
            await Client.Play(playlist.Id, position, ct);
            return;
        }

        if (NextBy != null)
        {
            await Client.PlayNextBy(NextBy, ct);
            return;
        }

        if (PreviousBy != null)
        {
            await Client.PlayPreviousBy(PreviousBy, ct);
            return;
        }

        if (Next)
        {
            await Client.PlayNext(ct);
            return;
        }

        if (Previous)
        {
            await Client.PlayPrevious(ct);
            return;
        }

        await Client.PlayCurrent(ct);
    }
}

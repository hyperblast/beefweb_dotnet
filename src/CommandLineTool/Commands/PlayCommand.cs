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
    [Option(T.Playlist, CommandOptionType.SingleValue, Description = D.Playlist)]
    public PlaylistRef? Playlist { get; set; }

    [Option(T.Index, Description = D.Index)]
    public int? Index { get; set; }

    [Option("-n|--next", Description = "Play next item")]
    public bool Next { get; set; }

    [Option("--next-by", Description = "Play next item by specified formatting expression (e.g. %album%)")]
    public string? NextBy { get; set; }

    [Option("-r|--previous", Description = "Play previous item")]
    public bool Previous { get; set; }

    [Option("--previous-by", Description = "Play previous item by specified formatting expression (e.g. %album%)")]
    public string? PreviousBy { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (Playlist != null)
        {
            if (Index == null)
            {
                throw new InvalidRequestException("Item index is required if playlist is specified.");
            }

            await Client.Play(Playlist.Value, Index.Value, ct);
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

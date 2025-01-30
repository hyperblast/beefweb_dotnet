using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("copy", "cp", Description = "Copy playlist items",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class CopyCommand(IClientProvider clientProvider, IConsole console)
    : TransferItemsCommandBase(clientProvider, console)
{
    protected override async Task ProcessAsync(PlaylistInfo playlist, IReadOnlyList<int> items, CancellationToken ct)
    {
        var (targetPlaylist, targetIndex) = GetTargetPlaylistAndIndex(playlist);
        await Client.CopyPlaylistItems(playlist.Id, items, targetPlaylist.Id, targetIndex, ct);
    }

    protected override async Task ProcessAllAsync(PlaylistInfo playlist, CancellationToken ct)
    {
        var (targetPlaylist, targetIndex) = GetTargetPlaylistAndIndex(playlist);
        var items = Enumerable.Range(0, playlist.ItemCount).ToArray();
        await Client.CopyPlaylistItems(playlist.Id, items, targetPlaylist.Id, targetIndex, ct);
    }
}

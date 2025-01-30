using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("delete", "del", "rm", Description = "Delete playlist items",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class DeleteCommand(IClientProvider clientProvider, IConsole console)
    : ItemsCommandBase(clientProvider, console)
{
    protected override async Task ProcessAsync(PlaylistInfo playlist, IReadOnlyList<int> items, CancellationToken ct)
    {
        await Client.RemovePlaylistItems(playlist.Id, items, ct);
    }

    protected override async Task ProcessAllAsync(PlaylistInfo playlist, CancellationToken ct)
    {
        await Client.ClearPlaylist(playlist.Id, ct);
    }
}

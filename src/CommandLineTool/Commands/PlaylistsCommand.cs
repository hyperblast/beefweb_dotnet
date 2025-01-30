using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("playlists", Description = "List or modify playlists")]
[Subcommand(typeof(PlaylistsAddCommand))]
[Subcommand(typeof(PlaylistsDeleteCommand))]
[Subcommand(typeof(PlaylistsModifyCommand))]
public class PlaylistsCommand(IClientProvider clientProvider, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var playlists = await Client.GetPlaylists(ct);

        var rows = playlists
            .Select(p =>
                new[]
                {
                    p.Id,
                    p.Title,
                    p.IsCurrent ? "(current)" : ""
                })
            .ToList();

        writer.WriteTable(rows);
    }
}

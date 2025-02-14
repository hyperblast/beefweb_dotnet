using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("playlists", Description = "List or modify playlists")]
[Subcommand(typeof(PlaylistsAddCommand))]
[Subcommand(typeof(PlaylistsDeleteCommand))]
[Subcommand(typeof(PlaylistsModifyCommand))]
public class PlaylistsCommand(IClientProvider clientProvider, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    [Option(T.ShowIndices, Description = D.ShowPlaylistIndices)]
    public bool ShowIndices { get; set; }

    [Option(T.ShowIdentifiers, Description = D.ShowPlaylistIdentifiers)]
    public bool ShowIdentifiers { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var playlists = await Client.GetPlaylists(ct);

        var playlistData = playlists.Select(GetPlaylistColumns);

        var rows = ShowIndices
            ? playlistData.ToTable(IndicesFrom0 ? 0 : 1, 1)
            : playlistData.ToTable();

        writer.WriteTable(rows, new WriteTableOptions { RightAlign = [ShowIndices] });
    }

    private IEnumerable<string> GetPlaylistColumns(PlaylistInfo p)
    {
        yield return p.IsCurrent ? "*" : " ";

        if (ShowIdentifiers)
        {
            yield return p.Id;
        }

        yield return p.Title;
    }
}

using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("delete", Description = "Delete playlist")]
public class PlaylistsDeleteCommand(IClientProvider clientProvider) : ServerCommandBase(clientProvider)
{
    [Argument(0, Description = D.Playlist)]
    [Required]
    public PlaylistRef Playlist { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        await Client.RemovePlaylist(Playlist, ct);
    }
}

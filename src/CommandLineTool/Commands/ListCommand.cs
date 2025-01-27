using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("list", Description = "List playlist items")]
public class ListCommand(IClientProvider clientProvider, ISettingsStorage storage, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    [Option(T.Playlist, CommandOptionType.SingleValue, Description = D.PlaylistToUse)]
    public PlaylistRef Playlist { get; set; } = PlaylistRef.Current;

    [Option(T.ItemIndex, Description = D.StartingItemIndex)]
    public int ItemIndex { get; set; } = 0;

    [Option("-n|--limit", Description = "Limit number of displayed items")]
    public int Limit { get; set; } = 100;

    [Option(T.ItemColumns, Description = D.PlaylistItemColumns)]
    public string[]? ItemColumns { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var columns = ItemColumns.GetOrDefault(storage.Settings.ListFormat);
        var result = await Client.GetPlaylistItems(Playlist, new PlaylistItemRange(ItemIndex, Limit), columns, ct);
        writer.WriteTable(result.Items.Select(i => i.Columns.ToArray()).ToArray());
    }
}

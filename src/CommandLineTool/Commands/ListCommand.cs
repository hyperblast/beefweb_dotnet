using System.Collections.Generic;
using System.Globalization;
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
    private static readonly bool[] ItemIndicesColumnAlign = [true];

    [Option(T.Playlist, CommandOptionType.SingleValue, Description = D.PlaylistToUse)]
    public PlaylistRef Playlist { get; set; } = PlaylistRef.Current;

    [Option(T.ItemColumns, Description = D.PlaylistItemColumns)]
    public string[]? ItemColumns { get; set; }

    [Option("-d|--indices", Description = "Display item indices")]
    public bool ShowIndices { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    [Argument(0, Description = "Playlist item range")]
    public string Range { get; set; } = "1..100";

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var columns = ItemColumns.GetOrDefault(storage.Settings.ListFormat);
        var range = ValueParser.ParseRange(Range, IndicesFrom0);

        var itemRange = range.GetItemRange(await Client.GetItemCount(Playlist, ct));
        if (itemRange.Count == 0)
        {
            return;
        }

        var result = await Client.GetPlaylistItems(Playlist, itemRange, columns, ct);
        var offset = itemRange.Offset + (IndicesFrom0 ? 0 : 1);

        var rows = ShowIndices
            ? result.Items.Select(
                (i, n) => (string[]) [(n + offset).ToString(CultureInfo.InvariantCulture), ..i.Columns])
            : result.Items.Select(i => i.Columns.ToArray());

        writer.WriteTable(rows.ToArray(), ShowIndices ? ItemIndicesColumnAlign : null);
    }
}

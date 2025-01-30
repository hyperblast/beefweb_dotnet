using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("list", "ls", Description = "List playlist items",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
public class ListCommand(IClientProvider clientProvider, ISettingsStorage storage, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    private static readonly bool[] ItemIndicesColumnAlign = [true];

    [Option(T.Playlist, Description = D.PlaylistToUse)]
    public string Playlist { get; set; } = Constants.CurrentPlaylist;

    [Option(T.ItemColumns, Description = D.PlaylistItemColumns)]
    public string[]? ItemColumns { get; set; }

    [Option("-n|--indices", Description = "Display item indices")]
    public bool ShowIndices { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var columns = ItemColumns.GetOrDefault(storage.Settings.ListFormat);
        var playlist = await Client.GetPlaylist(Playlist, IndicesFrom0, ct);
        var itemRange = new PlaylistItemRange(0, 100);

        if (RemainingArguments is { Length: > 0 })
        {
            var range = ValueParser.ParseRange(RemainingArguments[0], IndicesFrom0);
            itemRange = range.GetItemRange(playlist.ItemCount);

            if (itemRange.Count == 0)
            {
                return;
            }
        }

        var result = await Client.GetPlaylistItems(playlist.Id, itemRange, columns, ct);
        var offset = itemRange.Offset + (IndicesFrom0 ? 0 : 1);

        var rows = ShowIndices
            ? result.Items.Select(
                (i, n) => (string[]) [(n + offset).ToString(CultureInfo.InvariantCulture), ..i.Columns])
            : result.Items.Select(i => i.Columns.ToArray());

        writer.WriteTable(rows.ToArray(), ShowIndices ? ItemIndicesColumnAlign : null);
    }
}

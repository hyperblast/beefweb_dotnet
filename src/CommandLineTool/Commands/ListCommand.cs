using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("list", "ls",
    Description = "List playlist items",
    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue,
    ExtendedHelpText =
        "\nBy default first 100 items of the playlist are returned." +
        "\nArgument in format Start..End could be used to specify other range.")]
public class ListCommand(IClientProvider clientProvider, ISettingsStorage storage, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    [Option(T.Playlist, Description = D.PlaylistToUse)]
    public string Playlist { get; set; } = Constants.CurrentPlaylist;

    [Option(T.Format, Description = D.PlaylistItemsFormat)]
    public string[]? Columns { get; set; }

    [Option(T.ShowIndices, Description = D.ShowItemIndices)]
    public bool ShowIndices { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    [Option(T.Separator, Description = D.Separator)]
    public string Separator { get; set; } = " | ";

    public string[]? RemainingArguments { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var columns = Columns.GetOrDefault(storage.Settings.ListFormat);
        var playlist = await Client.GetPlaylist(Playlist, IndicesFrom0, ct);
        var itemRange = new PlaylistItemRange(0, 100);

        if (RemainingArguments is { Length: > 0 })
        {
            var range = RangeParser.Parse(RemainingArguments[0], IndicesFrom0);
            itemRange = range.GetItemRange(playlist.ItemCount);

            if (itemRange.Count == 0)
            {
                return;
            }
        }

        var result = await Client.GetPlaylistItems(playlist.Id, itemRange, columns, ct);
        var offset = itemRange.Offset + (IndicesFrom0 ? 0 : 1);

        var rows = ShowIndices
            ? result.Items.Select(i => i.Columns).ToTable(offset)
            : result.Items.Select(i => i.Columns).ToTable();

        writer.WriteTable(rows, new TableWriteOptions
        {
            RightAlign = [ShowIndices],
            Separator = Separator
        });
    }
}

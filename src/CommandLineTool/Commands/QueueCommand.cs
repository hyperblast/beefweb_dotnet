using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using static Beefweb.CommandLineTool.Commands.CommonOptions;

namespace Beefweb.CommandLineTool.Commands;

[Command("queue", "q", Description = "List or modify playback queue")]
[Subcommand(typeof(QueueAddCommand))]
[Subcommand(typeof(QueueDeleteCommand))]
public class QueueCommand(IClientProvider clientProvider, ISettingsStorage storage, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    [Option(T.Format, Description = D.PlaylistItemsFormat)]
    public string[]? Columns { get; set; }

    [Option(T.ShowIndices, Description = D.ShowQueueIndices)]
    public bool ShowIndices { get; set; }

    [Option(T.ShowIdentifiers, Description = D.ShowPlaylistIdentifiersAndItemIndices)]
    public bool ShowIdentifiers { get; set; }

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    [Option(T.Separator, Description = D.Separator)]
    public string? Separator { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var columns = Columns.GetOrDefault(storage.Settings.PlayQueueColumns);
        var queue = await Client.GetPlayQueue(columns, ct);
        var baseIndex = IndicesFrom0 ? 0 : 1;
        var data = queue.Select(q => GetItemColumns(q, baseIndex));

        var rows = ShowIndices
            ? data.ToTable(baseIndex)
            : data.ToTable();

        writer.WriteTable(rows, new TableWriteOptions
        {
            RightAlign = [ShowIndices],
            Separator = Separator.GetOrDefault(storage.Settings.ColumnSeparator)
        });
    }

    private IEnumerable<string> GetItemColumns(PlayQueueItemInfo item, int baseIndex)
    {
        if (ShowIdentifiers)
        {
            yield return FormattableString.Invariant($"{item.PlaylistId}:{item.ItemIndex + baseIndex}");
        }

        foreach (var column in item.Columns)
        {
            yield return column;
        }
    }
}

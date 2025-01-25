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

[Command("status", Description = "Display player status")]
public class StatusCommand(IClientProvider clientProvider, ITabularWriter writer, ISettingsStorage storage)
    : ServerCommandBase(clientProvider)
{
    [Option(T.TrackColumns, Description = D.TrackColumnsCurrent)]
    public string[]? TrackColumns { get; set; }

    [Option("-v|--volume", Description = "Display volume information")]
    public bool Volume { get; set; }

    [Option("-o|--options", Description = "Display player options")]
    public bool Options { get; set; }

    [Option("-p|--playlist", Description = "Display playlist information")]
    public bool Playlist { get; set; }

    [Option("-a|--all", Description = "Display all status information")]
    public bool All { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var formatExpressions = TrackColumns is { Length: > 0 }
            ? (IReadOnlyList<string>?)TrackColumns
            : storage.Settings.StatusFormat;

        var state = await Client.GetPlayerState(formatExpressions, ct);
        var activeItem = state.ActiveItem;
        var activeItemColumns = state.ActiveItem.Columns;

        var properties = new List<string[]>();

        if (state.PlaybackState == PlaybackState.Stopped)
        {
            properties.Add([state.PlaybackState.ToString()]);
        }
        else
        {
            properties.Add([state.PlaybackState.ToString(), ..activeItemColumns]);
        }

        if (Playlist || All)
        {
            properties.Add(["Playlist", activeItem.PlaylistId ?? ""]);
            properties.Add(["Index", activeItem.Index.ToString(CultureInfo.CurrentCulture)]);
        }

        if (Volume || All)
        {
            properties.Add(["Volume", state.Volume.Format()]);
            properties.Add(["IsMuted", state.Volume.IsMuted.ToString()]);
        }

        if (Options || All)
        {
            properties.AddRange(state.Options.Select(o => o.Format()));
        }

        writer.WriteTable(properties);
    }
}

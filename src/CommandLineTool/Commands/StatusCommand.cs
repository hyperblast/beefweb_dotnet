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
    [Option(T.ItemColumns, Description = D.CurrentItemColumns)]
    public string[]? ItemColumns { get; set; }

    [Option("-v|--volume", Description = "Display volume information")]
    public bool Volume { get; set; }

    [Option("-o|--options", Description = "Display player options")]
    public bool Options { get; set; }

    [Option(T.Playlist, Description = "Display playlist information")]
    public bool Playlist { get; set; }

    [Option("-r|--version", Description = "Display version information")]
    public bool Version { get; set; }

    [Option("-a|--all", Description = "Display all status information")]
    public bool All { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var columns = ItemColumns.GetOrDefault(storage.Settings.StatusFormat);
        var state = await Client.GetPlayerState(columns, ct);
        var activeItem = state.ActiveItem;

        var properties = new List<string[]>();

        if (state.PlaybackState == PlaybackState.Stopped)
        {
            properties.Add([state.PlaybackState.ToString()]);
        }
        else
        {
            properties.Add([state.PlaybackState.ToString(), ..activeItem.Columns]);
        }

        if (Playlist || All)
        {
            properties.Add(["Playlist", activeItem.PlaylistId ?? ""]);
            properties.Add(["Index", activeItem.Index.ToString(CultureInfo.InvariantCulture)]);
        }

        if (Volume || All)
        {
            properties.Add(["Volume", state.Volume.Format()]);
        }

        if (Options || All)
        {
            properties.Add([]);
            properties.Add(["Options:"]);
            properties.AddRange(state.Options.Select(o => o.Format()));
        }

        if (Version || All)
        {
            properties.Add([]);
            properties.Add(["Versions:"]);
            properties.Add([state.Info.Title, state.Info.Version]);
            properties.Add(["Plugin", state.Info.PluginVersion]);
        }

        writer.WriteTable(properties);
    }
}

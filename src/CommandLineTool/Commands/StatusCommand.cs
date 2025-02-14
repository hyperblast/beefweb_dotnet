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
    [Option(T.Format, Description = D.CurrentItemFormat)]
    public string? Format { get; set; }

    [Option("-p|--playback", Description = "Display playback information (default if no other option is specified)")]
    public bool Playback { get; set; }

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

    [Option(T.IndicesFrom0, Description = D.IndicesFrom0)]
    public bool IndicesFrom0 { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        var format = Format.GetOrDefault(storage.Settings.StatusFormat);
        var state = await Client.GetPlayerState([format], ct);
        var activeItem = state.ActiveItem;
        var baseIndex = IndicesFrom0 ? 0 : 1;
        var properties = new List<string[]>();

        if (Playback || All || !(Playlist || Volume || Options || Version))
        {
            var isStopped = state.PlaybackState == PlaybackState.Stopped;
            var track = isStopped ? "none" : activeItem.Columns[0];
            var position = isStopped ? "none" : activeItem.FormatProgress();

            properties.Add(["Playback:"]);
            properties.Add(["", "State", state.PlaybackState.ToString()]);
            properties.Add(["", "Track", track]);
            properties.Add(["", "Position", position]);
        }

        if (Playlist || All)
        {
            AddEmptyLine();
            properties.Add(["Playlist:"]);

            var playlistId = activeItem.PlaylistId ?? "none";

            var playlistIndex = activeItem.PlaylistIndex >= 0
                ? (activeItem.PlaylistIndex + baseIndex).ToString(CultureInfo.InvariantCulture)
                : "none";

            var itemIndex = activeItem.Index >= 0
                ? (activeItem.Index + baseIndex).ToString(CultureInfo.InvariantCulture)
                : "none";

            properties.Add(["", "Playlist id", playlistId]);
            properties.Add(["", "Playlist index", playlistIndex]);
            properties.Add(["", "Item index", itemIndex]);
        }

        if (Volume || All)
        {
            AddEmptyLine();
            properties.Add(["Volume:"]);
            properties.Add(["", "Value", state.Volume.Format()]);
        }

        if (Options || All)
        {
            AddEmptyLine();
            properties.Add(["Options:"]);
            properties.AddRange(state.Options.Select(o => (string[])
                ["", o.Name.CapitalizeFirstChar(), o.FormatValue(IndicesFrom0)]));
        }

        if (Version || All)
        {
            AddEmptyLine();
            properties.Add(["Versions:"]);
            properties.Add(["", state.Info.Title, state.Info.Version]);
            properties.Add(["", "Beefweb", state.Info.PluginVersion]);
        }

        writer.WriteTable(properties);
        return;

        void AddEmptyLine()
        {
            if (properties.Count > 0)
            {
                properties.Add([]);
            }
        }
    }
}

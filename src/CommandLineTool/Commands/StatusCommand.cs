using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("status", Description = "Display current status")]
public class StatusCommand(IClientProvider clientProvider, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    [Option("-t|--tf", Description = "Format current track using specified title formatting expressions")]
    public string[]? TitleFormats { get; set; }

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

        PlayerState state;
        IEnumerable<string> activeItemColumns;
        ActiveItemInfo activeItem;

        if (TitleFormats is { Length: > 0 })
        {
            state = await Client.GetPlayerState(TitleFormats, ct);
            activeItem = state.ActiveItem;
            activeItemColumns = state.ActiveItem.Columns;
        }
        else
        {
            state = await Client.GetPlayerState(["%artist% - %title%"], ct);
            activeItem = state.ActiveItem;
            activeItemColumns = [activeItem.Columns[0], activeItem.FormatProgress()];
        }

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
            foreach (var option in state.Options)
            {
                var value = option.Value is int intValue ? option.EnumNames![intValue] : option.Value.ToString();
                properties.Add([option.Name, value!]);
            }
        }

        writer.WriteTable(properties);
    }
}

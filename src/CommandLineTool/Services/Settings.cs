using System;
using System.Collections.Generic;

namespace Beefweb.CommandLineTool.Services;

public sealed class Settings
{
    public PredefinedServerCollection PredefinedServers { get; set; } = new();

    public string? DefaultServer { get; set; }

    public List<string> NowPlayingFormat { get; set; } = new();

    public List<string> StatusFormat { get; set; } = new();

    public List<string> ListFormat { get; set; } = new();

    public List<string> PlayQueueFormat { get; set; } = new();

    public bool IsDefaultServer(string name) => string.Equals(name, DefaultServer, StringComparison.OrdinalIgnoreCase);

    public static Settings CreateDefault()
    {
        return new Settings
        {
            DefaultServer = Constants.LocalServerName,
            PredefinedServers =
            {
                { Constants.LocalServerName, new Uri(Constants.LocalServerUrl) }
            },
            NowPlayingFormat = ["%artist% - %title%"],
            StatusFormat = ["%artist% - %title%"],
            ListFormat = ["%artist%", "%album%", "%title%"],
            PlayQueueFormat = ["%artist%", "%album%", "%title%"],
        };
    }
}

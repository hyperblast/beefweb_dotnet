using System;
using System.Collections.Generic;

namespace Beefweb.CommandLineTool.Services;

public sealed class Settings
{
    public PredefinedServerCollection PredefinedServers { get; set; } = new();

    public string? DefaultServer { get; set; }

    public string ColumnSeparator { get; set; } = "";

    public string NowPlayingFormat { get; set; } = "";

    public string StatusFormat { get; set; } = "";

    public List<string> ListColumns { get; set; } = new();

    public List<string> PlayQueueColumns { get; set; } = new();

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
            ColumnSeparator = " | ",
            NowPlayingFormat = "%artist% - %title%",
            StatusFormat = "%artist% - %title%",
            ListColumns = ["%artist%", "%album%", "%title%"],
            PlayQueueColumns = ["%artist%", "%album%", "%title%"],
        };
    }
}

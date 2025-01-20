using System;

namespace Beefweb.CommandLineTool.Services;

public sealed class Settings
{
    public PredefinedServerCollection PredefinedServers { get; set; } = new();

    public string? DefaultServer { get; set; }

    public static Settings CreateDefault()
    {
        return new Settings
        {
            DefaultServer = Constants.LocalServerName,
            PredefinedServers =
            {
                { Constants.LocalServerName, new Uri(Constants.LocalServerUrl) }
            }
        };
    }
}

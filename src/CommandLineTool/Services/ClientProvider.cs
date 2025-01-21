using System;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public interface IClientProvider
{
    string? ServerName { get; set; }

    IPlayerClient Client { get; }
}

public sealed class ClientProvider(ISettingsStorage settingsStorage) : IClientProvider, IDisposable
{
    private PlayerClient? _client;

    public string? ServerName { get; set; }

    public IPlayerClient Client => _client ??= new PlayerClient(GetServerUri());

    private Uri GetServerUri()
    {
        if (Uri.TryCreate(ServerName, UriKind.Absolute, out var customUri))
        {
            if (UriValidator.HasHttpScheme(customUri))
            {
                throw new InvalidRequestException(Messages.HttpUrlRequired);
            }

            return customUri;
        }

        var settings = settingsStorage.Settings;
        var serverName = ServerName ?? settings.DefaultServer ??
            throw new InvalidRequestException("No server is specified and no default server is configured.");

        if (settings.PredefinedServers.TryGetValue(serverName, out var predefinedUri))
        {
            return predefinedUri;
        }

        throw new InvalidRequestException($"Unknown predefined server '{serverName}'.");
    }

    public void Dispose() => _client?.Dispose();
}

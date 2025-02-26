namespace Beefweb.Client;

/// <summary>
/// Specifies which operations are allowed by current configuration.
/// </summary>
public sealed class ApiPermissions
{
    /// <summary>
    /// True, if changing playlists is allowed.
    /// </summary>
    public bool ChangePlaylists { get; set; }

    /// <summary>
    /// True, if changing output device is allowed.
    /// </summary>
    public bool ChangeOutput { get; set; }

    /// <summary>
    /// True, if changing client configuration is allowed.
    /// </summary>
    public bool ChangeClientConfig { get; set; }
}

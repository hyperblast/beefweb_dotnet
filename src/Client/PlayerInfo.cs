namespace Beefweb.Client;

/// <summary>
/// Player information.
/// </summary>
public sealed class PlayerInfo
{
    /// <summary>
    /// Short player name, e.g. 'deadbeef' or 'foobar2000'.
    /// This is useful for identification of current player.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Player title suitable for displaying to user.
    /// Unlike <see cref="Name"/> this can have arbitrary value.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Version of the player.
    /// </summary>
    public string Version { get; set; } = null!;

    /// <summary>
    /// Version of the Beefweb plugin.
    /// </summary>
    public string PluginVersion { get; set; } = null!;
}
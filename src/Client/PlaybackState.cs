namespace Beefweb.Client;

/// <summary>
/// Playback state.
/// </summary>
public enum PlaybackState
{
    /// <summary>
    /// Player is stopped.
    /// </summary>
    Stopped,

    /// <summary>
    /// Player is currently playing something.
    /// </summary>
    Playing,

    /// <summary>
    /// Player is paused.
    /// </summary>
    Paused
}
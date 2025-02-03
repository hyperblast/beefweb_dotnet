using System.Text.Json.Serialization;

namespace Beefweb.Client;

/// <summary>
/// Player event.
/// </summary>
public sealed class PlayerEvent
{
    /// <summary>
    /// Player state changed. Use <see cref="IPlayerClient.GetPlayerState"/> to query new state.
    /// </summary>
    [JsonPropertyName("player")]
    public bool PlayerChanged { get; set; }

    /// <summary>
    /// Playlists are changed. Use <see cref="IPlayerClient.GetPlaylists"/> to query new state.
    /// </summary>
    [JsonPropertyName("playlists")]
    public bool PlaylistsChanged { get; set; }

    /// <summary>
    /// Playlist items are changed. Use <see cref="IPlayerClient.GetPlaylistItems"/> to query new state.
    /// </summary>
    [JsonPropertyName("playlistItems")]
    public bool PlaylistItemsChanged { get; set; }

    /// <summary>
    /// Playback queue is changed. Use <see cref="IPlayerClient.GetPlayQueue"/> to query new state.
    /// </summary>
    [JsonPropertyName("playQueue")]
    public bool PlayQueueChanged { get; set; }
}

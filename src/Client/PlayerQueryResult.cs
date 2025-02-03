using System.Collections;
using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Result of player query.
/// </summary>
public sealed class PlayerQueryResult
{
    /// <summary>
    /// Current player state.
    /// </summary>
    public PlayerState? Player { get; set; }

    /// <summary>
    /// All playlists.
    /// </summary>
    public IList<PlaylistInfo>? Playlists { get; set; }

    /// <summary>
    /// Requested playlist items.
    /// </summary>
    public PlaylistItemsResult? PlaylistItems { get; set; }

    /// <summary>
    /// Play queue contents.
    /// </summary>
    public IList<PlayQueueItemInfo>? PlayQueue { get; set; }
}

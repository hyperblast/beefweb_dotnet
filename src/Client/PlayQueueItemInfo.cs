using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Play queue item information.
/// </summary>
public sealed class PlayQueueItemInfo
{
    /// <summary>
    /// Playlist index.
    /// </summary>
    public int PlaylistIndex { get; set; }

    /// <summary>
    /// Playlist id.
    /// </summary>
    public string PlaylistId { get; set; } = null!;

    /// <summary>
    /// Item index in playlist.
    /// </summary>
    public int ItemIndex { get; set; }

    /// <summary>
    /// Requested item columns.
    /// </summary>
    public IList<string> Columns { get; set; } = null!;
}

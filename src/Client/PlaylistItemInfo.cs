using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Playlist item information.
/// </summary>
public sealed class PlaylistItemInfo
{
    /// <summary>
    /// Requested item columns.
    /// </summary>
    public IList<string> Columns { get; set; } = null!;
}
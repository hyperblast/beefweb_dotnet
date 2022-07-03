using System.Collections.Generic;

namespace Beefweb.Client
{
    /// <summary>
    /// Playlist items result.
    /// </summary>
    public sealed class PlaylistItemsResult
    {
        /// <summary>
        /// Playlist offset originally requested.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Total number of items in the playlist.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Requested playlist items information.
        /// </summary>
        public IList<PlaylistItemInfo> Items { get; set; } = null!;
    }
}

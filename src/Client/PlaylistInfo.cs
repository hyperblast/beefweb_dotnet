using System;

namespace Beefweb.Client
{
    /// <summary>
    /// Playlist information.
    /// </summary>
    public sealed class PlaylistInfo
    {
        /// <summary>
        /// Playlist identifier.
        /// This value is stable and preserved if playlists are rearranged.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Playlist index.
        /// This value might change if playlists are rearranged.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Playlist title.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Is playlist currently selected in player UI.
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Number of items in the playlists.
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Total time of all playlist items.
        /// This value might be <see cref="TimeSpan.Zero"/> for certain players.
        /// </summary>
        public TimeSpan TotalTime { get; set; }
    }
}

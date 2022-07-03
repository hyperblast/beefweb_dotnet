using System;
using System.Collections.Generic;

namespace Beefweb.Client
{
    /// <summary>
    /// Information about track currently being played.
    /// </summary>
    public sealed class ActiveItemInfo
    {
        /// <summary>
        /// Playlist identifier of the currently played track.
        /// This value might be null, if track does not belong to any playlist.
        /// </summary>
        public string? PlaylistId { get; set; }

        /// <summary>
        /// Playlist index of the currently played track.
        /// This value might be -1, if track does not belong to any playlist.
        /// </summary>
        public int PlaylistIndex { get; set; }

        /// <summary>
        /// Index of currently played track in the playlist
        /// identified by <see cref="PlaylistId"/> and <see cref="PlaylistIndex"/>.
        /// This value might be -1, if track does not belong to any playlist.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Current playback position.
        /// </summary>
        public TimeSpan Position { get; set; }

        /// <summary>
        /// Duration of the current track.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Requested columns of the current track.
        /// </summary>
        public IList<string> Columns { get; set; } = null!;
    }
}

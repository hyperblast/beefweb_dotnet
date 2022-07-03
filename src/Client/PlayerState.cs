using System.Collections.Generic;

namespace Beefweb.Client
{
    /// <summary>
    /// Player state.
    /// </summary>
    public sealed class PlayerState
    {
        /// <summary>
        /// Information about current player.
        /// </summary>
        public PlayerInfo Info { get; set; } = null!;

        /// <summary>
        /// Information about track being played.
        /// </summary>
        public ActiveItemInfo ActiveItem { get; set; } = null!;

        /// <summary>
        /// Current playback state.
        /// </summary>
        public PlaybackState PlaybackState { get; set; }

        /// <summary>
        /// Current playback mode.
        /// This value is an index in <see cref="PlaybackModes"/> collection.
        /// This value can be -1 if playback modes are not available for the current player.
        /// </summary>
        public int PlaybackMode { get; set; }

        /// <summary>
        /// List of player playback modes. This collection can be empty.
        /// </summary>
        public IList<string> PlaybackModes { get; set; } = null!;

        /// <summary>
        /// Information about playback volume.
        /// </summary>
        public VolumeInfo Volume { get; set; } = null!;
    }
}

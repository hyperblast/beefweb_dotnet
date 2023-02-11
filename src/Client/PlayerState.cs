using System;
using System.Collections.Generic;
using System.Linq;

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
        [Obsolete("Use Options")]
        public int PlaybackMode { get; set; }

        /// <summary>
        /// List of player playback modes. This collection can be empty.
        /// </summary>
        [Obsolete("Use Options")]
        public IList<string> PlaybackModes { get; set; } = null!;

        /// <summary>
        /// List of player options.
        /// </summary>
        public IList<PlayerOption> Options { get; set; } = null!;

        /// <summary>
        /// Information about playback volume.
        /// </summary>
        public VolumeInfo Volume { get; set; } = null!;

        /// <summary>
        /// Gets option with specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Option id.</param>
        /// <returns>Found option.</returns>
        /// <exception cref="KeyNotFoundException">No option with <paramref name="id"/> was found.</exception>
        public PlayerOption GetOption(string id)
        {
            return Options.FirstOrDefault(o => string.Equals(o.Id, id, StringComparison.OrdinalIgnoreCase))
                   ?? throw new KeyNotFoundException($"Option with id '{id}' is not found.");
        }
    }
}

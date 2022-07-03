using System;

namespace Beefweb.Client
{
    /// <summary>
    /// Request to update player state.
    /// Any null value means "don't change this property".
    /// </summary>
    public sealed class UpdatePlayerStateRequest
    {
        /// <summary>
        /// New volume.
        /// </summary>
        public decimal? Volume { get; set; }

        /// <summary>
        /// New mute state.
        /// </summary>
        public BoolSwitch? IsMuted { get; set; }

        /// <summary>
        /// New position from the beginning of the track.
        /// </summary>
        public TimeSpan? Position { get; set; }

        /// <summary>
        /// New position relative to current position.
        /// </summary>
        public TimeSpan? RelativePosition { get; set; }

        /// <summary>
        /// Playback mode. Value should be index of item in <see cref="PlayerState.PlaybackModes"/> collection.
        /// </summary>
        public int? PlaybackMode { get; set; }
    }
}

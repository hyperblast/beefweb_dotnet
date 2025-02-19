using System;
using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Request to modify player state.
/// Any null value means "don't change this property".
/// </summary>
public sealed class SetPlayerStateRequest
{
    /// <summary>
    /// New volume value.
    /// </summary>
    public double? Volume { get; set; }

    /// <summary>
    /// Adjust volume in one step: 1 for volume increase, -1 for volume decrease.
    /// </summary>
    public int? VolumeStep { get; set; }

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
    /// Playback mode. Value should be an index of item in <see cref="PlayerState.PlaybackModes"/> collection.
    /// </summary>
    [Obsolete("Use Options")]
    public int? PlaybackMode { get; set; }

    /// <summary>
    /// Requests to set option values.
    /// </summary>
    public IList<SetOptionRequest>? Options { get; set; }
}

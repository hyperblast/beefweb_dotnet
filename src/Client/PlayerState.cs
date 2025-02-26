using System;
using System.Collections.Generic;

namespace Beefweb.Client;

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
    /// Current permissions.
    /// </summary>
    /// <remarks>
    /// This property is available since Beefweb v0.10
    /// For earlier value is null.
    /// </remarks>
    public ApiPermissions? Permissions { get; set; }

    /// <summary>
    /// Gets option with specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Option id.</param>
    /// <returns>Found option.</returns>
    /// <exception cref="KeyNotFoundException">No option with <paramref name="id"/> was found.</exception>
    public PlayerOption GetOption(string id)
    {
        for (var i = 0; i < Options.Count; i++)
        {
            var option = Options[i];
            if (string.Equals(option.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                return option;
            }
        }

        throw new KeyNotFoundException($"Option with id '{id}' is not found.");
    }
}

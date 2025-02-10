using System;
using System.Globalization;
using Beefweb.Client.Infrastructure;

namespace Beefweb.Client;

/// <summary>
/// Playlist reference. Playlist can be identified by either <see cref="Id"/> or <see cref="Index"/>.
/// Playlist indices change when playlists are rearranged.
/// Playlist identifiers are stable.
/// </summary>
public readonly struct PlaylistRef : IEquatable<PlaylistRef>
{
    /// <summary>
    /// <see cref="PlaylistRef"/> referencing currently selected playlist.
    /// </summary>
    public static PlaylistRef Current { get; } = new(false, "current");

    /// <summary>
    /// Playlist identifier.
    /// If <see cref="Index"/> is zero or greater this value is null.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// Playlist index.
    /// If <see cref="Id"/> is not null, this value is -1.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Creates new instance from <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Playlist identifier</param>
    /// <exception cref="ArgumentException"><paramref name="id"/> is null or contains only whitespace characters.</exception>
    public PlaylistRef(string id)
    {
        ArgumentValidator.ValidatePlaylistId(id);

        Id = id;
        Index = -1;
    }

    /// <summary>
    /// Creates new instance from <paramref name="index"/>
    /// </summary>
    /// <param name="index">Playlist index.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative.</exception>
    public PlaylistRef(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        Id = null;
        Index = index;
    }

    private PlaylistRef(bool unused, int index)
    {
        Id = null;
        Index = index;
    }

    private PlaylistRef(bool unused, string id)
    {
        Id = id;
        Index = -1;
    }

    /// <summary>
    /// Implicitly converts <paramref name="index"/> to <see cref="PlaylistRef"/>.
    /// </summary>
    /// <param name="index">Playlist index.</param>
    /// <returns>Created playlist reference.</returns>
    public static implicit operator PlaylistRef(int index) => new(index);

    /// <summary>
    /// Implicitly converts <paramref name="id"/> to <see cref="PlaylistRef"/>.
    /// </summary>
    /// <param name="id">Playlist identifier.</param>
    /// <returns>Created playlist reference.</returns>
    public static implicit operator PlaylistRef(string id) => new(id);

    /// <inheritdoc />
    public override string ToString()
    {
        return Id ?? Index.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets raw value (either <see cref="string"/> or <see cref="int"/>).
    /// </summary>
    /// <returns></returns>
    public object GetValue()
    {
        return Id ?? (object)Index;
    }

    /// <summary>
    /// Creates <see cref="PlaylistRef"/> from string representation.
    /// </summary>
    /// <param name="value">String representation of playlist reference.</param>
    /// <returns>Parsed value of <paramref name="value"/>.</returns>
    public static PlaylistRef Parse(string value)
    {
        if (TryParse(value, out var playlistRef))
            return playlistRef;

        throw new ArgumentException($"Invalid playlist reference '{value}'.", nameof(value));
    }

    /// <summary>
    /// Tries to create <see cref="PlaylistRef"/> from string representation.
    /// </summary>
    /// <param name="value">String representation of playlist reference.</param>
    /// <param name="playlistRef">Parsed value.</param>
    /// <returns>Parsed value of <paramref name="value"/>.</returns>
    public static bool TryParse(string? value, out PlaylistRef playlistRef)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
        {
            if (index < 0)
            {
                playlistRef = default;
                return false;
            }

            playlistRef = new PlaylistRef(false, index);
            return true;
        }

        if (value != null && ArgumentValidator.IdMatcher().IsMatch(value))
        {
            playlistRef = new PlaylistRef(false, value);
            return true;
        }

        playlistRef = default;
        return false;
    }

    /// <inheritdoc />
    public bool Equals(PlaylistRef other) => Id == other.Id && Index == other.Index;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is PlaylistRef other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Id, Index);

    /// <summary>
    /// Checks if two instances are considered equal.
    /// </summary>
    /// <param name="left">First instance to compare.</param>
    /// <param name="right">Second instance to compare.</param>
    /// <returns>Result of the comparison.</returns>
    public static bool operator ==(PlaylistRef left, PlaylistRef right) => left.Equals(right);

    /// <summary>
    /// Checks if two instance are considered non-equal.
    /// </summary>
    /// <param name="left">First instance to compare.</param>
    /// <param name="right">Second instance to compare.</param>
    /// <returns>Result of the comparison.</returns>
    public static bool operator !=(PlaylistRef left, PlaylistRef right) => !left.Equals(right);
}

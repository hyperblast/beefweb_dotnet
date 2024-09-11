using System;
using System.Globalization;

namespace Beefweb.Client;

/// <summary>
/// Playlist reference. Playlist can be identified by either <see cref="Id"/> or <see cref="Index"/>.
/// Playlist indices change when playlists are rearranged.
/// Playlist identifiers are stable.
/// </summary>
public readonly struct PlaylistRef : IEquatable<PlaylistRef>
{
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
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid playlist identifier.", nameof(id));

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
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            Id = null;
            Index = index;
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
    /// Creates <see cref="PlaylistRef"/> from string representation.
    /// </summary>
    /// <param name="value">String representation of playlist reference.</param>
    /// <returns>Parsed value of <paramref name="value"/>.</returns>
    public static PlaylistRef Parse(string value)
    {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index)
                ? new PlaylistRef(index)
                : new PlaylistRef(value);
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
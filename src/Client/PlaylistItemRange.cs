using System;

namespace Beefweb.Client;

/// <summary>
/// Playlist item range.
/// </summary>
public readonly struct PlaylistItemRange : IEquatable<PlaylistItemRange>
{
    /// <summary>
    /// Creates new instance with specified <paramref name="offset"/> and <paramref name="count"/>.
    /// </summary>
    /// <param name="offset">Zero-based starting index in the playlist.</param>
    /// <param name="count">Maximal number of items to return.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is negative or <paramref name="count"/> is negative.</exception>
    public PlaylistItemRange(int offset, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        Offset = offset;
        Count = count;
    }

    /// <summary>
    /// Zero-based starting index in the playlist.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    /// Maximal number of playlist items to return.
    /// </summary>
    public int Count { get; }

    /// <inheritdoc />
    public override string ToString() => FormattableString.Invariant($"{Offset}:{Count}");

    /// <inheritdoc />
    public bool Equals(PlaylistItemRange other) => Offset == other.Offset && Count == other.Count;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is PlaylistItemRange other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Offset, Count);

    /// <summary>
    /// Checks if two instances are considered equal.
    /// </summary>
    /// <param name="left">First instance to compare.</param>
    /// <param name="right">Second instance to compare.</param>
    /// <returns>Result of the comparison.</returns>
    public static bool operator ==(PlaylistItemRange left, PlaylistItemRange right) => left.Equals(right);

    /// <summary>
    /// Checks if two instance are considered non-equal.
    /// </summary>
    /// <param name="left">First instance to compare.</param>
    /// <param name="right">Second instance to compare.</param>
    /// <returns>Result of the comparison.</returns>
    public static bool operator !=(PlaylistItemRange left, PlaylistItemRange right) => !left.Equals(right);
}

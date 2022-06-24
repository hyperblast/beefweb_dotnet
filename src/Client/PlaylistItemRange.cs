using System;

namespace Beefweb.Client
{
    public readonly struct PlaylistItemRange : IEquatable<PlaylistItemRange>
    {
        public PlaylistItemRange(int offset, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Offset = offset;
            Count = count;
        }

        public int Offset { get; }

        public int Count { get; }

        public override string ToString() => FormattableString.Invariant($"{Offset}:{Count}");

        public bool Equals(PlaylistItemRange other) => Offset == other.Offset && Count == other.Count;

        public override bool Equals(object? obj) => obj is PlaylistItemRange other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Offset, Count);

        public static bool operator ==(PlaylistItemRange left, PlaylistItemRange right) => left.Equals(right);

        public static bool operator !=(PlaylistItemRange left, PlaylistItemRange right) => !left.Equals(right);
    }
}

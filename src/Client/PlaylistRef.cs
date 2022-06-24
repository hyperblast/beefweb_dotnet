using System;
using System.Globalization;

namespace Beefweb.Client
{
    public readonly struct PlaylistRef : IEquatable<PlaylistRef>
    {
        public string? Id { get; }

        public int Index { get; }

        public PlaylistRef(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid playlist identifier.", nameof(id));

            Id = id;
            Index = -1;
        }

        public PlaylistRef(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            Id = null;
            Index = index;
        }

        public static implicit operator PlaylistRef(int index) => new PlaylistRef(index);

        public static implicit operator PlaylistRef(string id) => new PlaylistRef(id);

        public override string ToString()
        {
            return Id ?? Index.ToString(CultureInfo.InvariantCulture);
        }

        public static PlaylistRef Parse(string value)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index)
                ? new PlaylistRef(index)
                : new PlaylistRef(value);
        }

        public bool Equals(PlaylistRef other) => Id == other.Id && Index == other.Index;

        public override bool Equals(object? obj) => obj is PlaylistRef other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id, Index);

        public static bool operator ==(PlaylistRef left, PlaylistRef right) => left.Equals(right);

        public static bool operator !=(PlaylistRef left, PlaylistRef right) => !left.Equals(right);
    }
}

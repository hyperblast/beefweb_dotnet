using System;

namespace Beefweb.Client
{
    public sealed class PlaylistInfo
    {
        public string Id { get; set; } = null!;

        public int Index { get; set; }

        public string Title { get; set; } = null!;

        public bool IsCurrent { get; set; }

        public int ItemCount { get; set; }

        public TimeSpan TotalTime { get; set; }
    }
}

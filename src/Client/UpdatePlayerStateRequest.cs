using System;

namespace Beefweb.Client
{
    public sealed class UpdatePlayerStateRequest
    {
        public decimal? Volume { get; set; }

        public BoolSwitch? IsMuted { get; set; }

        public TimeSpan? Position { get; set; }

        public TimeSpan? RelativePosition { get; set; }

        public int? PlaybackMode { get; set; }
    }
}
using System.Collections.Generic;

namespace Beefweb.Client
{
    public sealed class PlayerState
    {
        public PlayerInfo Info { get; set; } = null!;

        public ActiveItemInfo ActiveItem { get; set; } = null!;

        public PlaybackState PlaybackState { get; set; }

        public int PlaybackMode { get; set; }

        public IList<string> PlaybackModes { get; set; } = null!;

        public VolumeInfo Volume { get; set; } = null!;
    }
}

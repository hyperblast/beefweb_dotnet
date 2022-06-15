using System.Collections.Generic;

namespace Beefweb.Client
{
    public sealed class PlayerQueryResult
    {
        public PlayerState? Player { get; set; }

        public IList<PlaylistInfo>? Playlists { get; set; }

        public PlaylistItemsResult? PlaylistItems { get; set; }
    }
}
using System.Collections.Generic;

namespace Beefweb.Client
{
    public sealed class PlaylistItemsResult
    {
        public int Offset { get; set; }

        public int TotalCount { get; set; }

        public IList<PlaylistItemInfo> Items { get; set; } = new List<PlaylistItemInfo>();
    }
}
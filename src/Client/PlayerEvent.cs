using System.Text.Json.Serialization;

namespace Beefweb.Client
{
    public sealed class PlayerEvent
    {
        [JsonPropertyName("player")]
        public bool PlayerChanged { get; set; }

        [JsonPropertyName("playlists")]
        public bool PlaylistsChanged { get; set; }

        [JsonPropertyName("playlistItems")]
        public bool PlaylistItemsChanged { get; set; }
    }
}
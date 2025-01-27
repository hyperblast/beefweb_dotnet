namespace Beefweb.CommandLineTool.Commands;

public static class CommonOptions
{
    public static class T
    {
        public const string Playlist = "-p|--playlist";
        public const string ItemIndex = "-i|--item";
        public const string ItemColumns = "-t|--tf";
        public const string Set = "-v|--set";
        public const string Position = "-P|--position";
    }

    public static class D
    {
        public const string PlaylistToUse = "Playlist to use (0-based index, id or current)";
        public const string PlaylistToDelete = "Playlist to delete (0-based index, id or current)";
        public const string PlaylistToAddTo = "Playlist to add to (0-based index, id or current)";
        public const string ItemIndex = "Playlist item index (0-based)";
        public const string StartingItemIndex = "Starting item index (0-based)";
        public const string CurrentItemColumns = "Format current track using specified title formatting expressions";
        public const string PlaylistItemColumns = "Format playlist items usign specified title formatting expressions";
        public const string PositionForPlaylist = "Position to insert playlist at";
        public const string PositionForItems = "Position to insert items at";
    }
}

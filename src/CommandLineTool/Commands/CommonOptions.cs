namespace Beefweb.CommandLineTool.Commands;

public static class CommonOptions
{
    public static class T
    {
        public const string Playlist = "-l|--playlist";
        public const string ItemIndex = "-i|--item";
        public const string ItemColumns = "-t|--tf";
        public const string Set = "-s|--set";
        public const string Position = "-p|--position";
        public const string Count = "-n|--count";
        public const string Stdin = "-I|--stdin";
        public const string Server = "-S|--server";
        public const string IndicesFrom0 = "-z|--zero";
    }

    public static class D
    {
        public const string PlaylistToUse = "Playlist to use (index, id or current)";
        public const string PlaylistToDelete = "Playlist to delete (index, id or current)";
        public const string PlaylistToAddTo = "Playlist to add to (index, id or current)";
        public const string ItemIndex = "Playlist item index";
        public const string CurrentItemColumns = "Format current track using specified title formatting expressions";
        public const string PlaylistItemColumns = "Format playlist items usign specified title formatting expressions";
        public const string PositionForPlaylist = "Position to insert playlist at";
        public const string PositionForItems = "Position to insert items at";
        public const string StdinItems = "Read items from standard input";
        public const string StdinIndices = "Read indices from standard input";
        public const string Server = "Server to use. Could be a server URL or name of the predefined server";
        public const string IndicesFrom0 = "Start counting indices from 0";
    }
}

namespace Beefweb.CommandLineTool.Commands;

public static class CommonOptions
{
    public static class T
    {
        public const string Playlist = "-l|--playlist";
        public const string TargetPlaylist = "-t|--to-playlist";
        public const string ItemColumns = "-t|--tf";
        public const string Set = "-s|--set";
        public const string Adjust = "-a|--adjust";
        public const string Position = "-p|--position";
        public const string Title = "-t|--title";
        public const string Select = "-s|--select";
        public const string Stdin = "-I|--stdin";
        public const string Server = "-S|--server";
        public const string IndicesFrom0 = "-z|--zero";
        public const string ShowIndices = "-n|--indices";
    }

    public static class D
    {
        public const string PlaylistToUse = "Playlist to use (index, id or current)";
        public const string TargetPlaylist = "Target playlist, if not specified the same value as --playlist is used";
        public const string PlaylistToAddTo = "Playlist to add to (index, id or current)";
        public const string CurrentItemColumns = "Format current track using specified title formatting expressions";
        public const string PlaylistItemColumns = "Format playlist items using specified title formatting expressions";
        public const string PositionForPlaylist = "Position to insert playlist at";
        public const string PositionForItems = "Position to insert items at";
        public const string StdinItems = "Read items from standard input";
        public const string StdinItemRanges = "Read item indices or ranges from standard input";
        public const string Server = "Server to use. Could be a server URL or name of the predefined server";
        public const string IndicesFrom0 = "Start counting indices from 0";
        public const string PlaylistTitle = "Playlist title";
        public const string Select = "Select playlist";
        public const string ShowItemIndices = "Show item indices";
        public const string ShowPlaylistIndices = "Show playlist indices";
    }
}

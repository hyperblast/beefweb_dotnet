namespace Beefweb.CommandLineTool.Commands;

public static class CommonOptions
{
    public static class T
    {
        public const string Playlist = "-p|--playlist";
        public const string Index = "-i|--item";
        public const string TrackColumns = "-t|--tf";
        public const string Value = "-v|--value|--set";
    }

    public static class D
    {
        public const string Playlist = "Playlist to use (0-based index, id or current)";
        public const string Index = "Playlist item index (0-based)";
        public const string TrackColumnsCurrent = "Format current track using specified title formatting expressions";
    }
}

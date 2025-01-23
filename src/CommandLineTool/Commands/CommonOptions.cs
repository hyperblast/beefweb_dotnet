namespace Beefweb.CommandLineTool.Commands;

public static class CommonOptions
{
    public static class T
    {
        public const string Playlist = "-p|--playlist";
        public const string Index = "-i|--item";
    }

    public static class D
    {
        public const string Playlist = "Playlist to use (0-based index, id or current)";
        public const string Index = "Playlist item index (0-based)";
    }
}

using System;

namespace Beefweb.CommandLineTool.Commands;

public static class CommonOptions
{
    public static class T
    {
        public const string Playlist = "-l|--playlist";
        public const string TargetPlaylist = "-t|--to-playlist";
        public const string Format = "-t|--tf";
        public const string Set = "-s|--set";
        public const string Adjust = "-a|--adjust";
        public const string Position = "-p|--position";
        public const string Title = "-t|--title";
        public const string Select = "-s|--select";
        public const string Stdin = "-I|--stdin";
        public const string Server = "-S|--server";
        public const string IndicesFrom0 = "-z|--zero";
        public const string ShowIndices = "-n|--indices";
        public const string ShowIdentifiers = "-d|--id";
        public const string TrackIndex = "-t|--track";
        public const string AllowEmptyInput = "-e|--allow-empty";
        public const string Separator = "-s|--separator";
    }

    public static class D
    {
        public const string PlaylistToUse = "Playlist to use (index, id or current)";
        public const string TargetPlaylist = "Target playlist, if not specified the same value as --playlist is used";
        public const string PlaylistToAddTo = "Playlist to add to (index, id or current)";
        public const string CurrentItemFormat = "Format current track using specified title formatting expression";
        public const string PlaylistItemsFormat = "Format playlist items using specified title formatting expressions";
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
        public const string ShowQueueIndices = "Show playback queue indices";
        public const string ShowPlaylistIdentifiers = "Show playlist identifiers";
        public const string ShowPlaylistIdentifiersAndItemIndices = "Show playlist identifiers and item indices";
        public const string PlayTrackIndex = "Play track at specified index";
        public const string DeleteTrackAtQueueIndex = "Delete track at specified queue index";
        public const string DeleteTrackAtPlaylistIndex = "Delete track at specified playlist index";
        public const string AllowEmptyInput = "Do not fail if no input items are specified";
        public const string Separator = "Separate columns using specified character";

        public const string ItemsCommandHelpDetails =
            "\nItems could be specified as either command line arguments or stdin stream (if -I is specified)." +
            "\nPositive number is intepreted as playlist item position (counting from 1 by default unless -z is specified)." +
            "\nNegative index means playlist position from the end (e.g. -1 for last playlist item)." +
            "\nItem indices could also be specified in range format Start..End (inclusive)." +
            "\nStart and End parts are optional, if they are not specified 1 and -1 are assumed respectively.";
    }
}

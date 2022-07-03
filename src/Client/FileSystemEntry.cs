using System;

namespace Beefweb.Client
{
    /// <summary>
    /// File system entry information
    /// </summary>
    public sealed class FileSystemEntry
    {
        /// <summary>
        /// Short file system entry name (e.g. MyTrac.flac).
        /// For file system roots this includes full path, e.g. C:\Music
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// File path to file (e.g. C:\MyTrac.flac).
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// File size. For directories this has value -1.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// File system entry type.
        /// </summary>
        public FileSystemEntryType Type { get; set; }

        /// <summary>
        /// Last modification timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}

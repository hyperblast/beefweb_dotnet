using System;

namespace Beefweb.Client
{
    public sealed class FileSystemEntry
    {
        public string Name { get; set; } = null!;

        public string Path { get; set; } = null!;

        public long Size { get; set; }

        public FileSystemEntryType Type { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
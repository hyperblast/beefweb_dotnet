using System.Collections.Generic;

namespace Beefweb.Client
{
    public sealed class FileSystemEntriesResult
    {
        public string PathSeparator { get; set; } = null!;

        public IList<FileSystemEntry> Entries { get; set; } = null!;
    }
}

using System.Collections.Generic;

namespace Beefweb.Client
{
    public sealed class FileSystemRootsResult
    {
        public string PathSeparator { get; set; } = null!;

        public IList<FileSystemEntry> Roots { get; set; } = null!;
    }
}

using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Result of file system roots enumeration.
/// </summary>
public sealed class FileSystemRootsResult
{
    /// <summary>
    /// File system separator. Typically, '/' or '\'.
    /// This value is the same as <see cref="FileSystemEntriesResult.PathSeparator"/>.
    /// </summary>
    public char PathSeparator { get; set; }

    /// <summary>
    /// List of file system roots.
    /// </summary>
    public IList<FileSystemEntry> Roots { get; set; } = null!;
}

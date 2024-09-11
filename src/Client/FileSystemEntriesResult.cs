using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Result of file system enumeration.
/// </summary>
public sealed class FileSystemEntriesResult
{
    /// <summary>
    /// File system separator. Typically '/' or '\'.
    /// This value is the same as <see cref="FileSystemRootsResult.PathSeparator"/>.
    /// </summary>
    public string PathSeparator { get; set; } = null!;

    /// <summary>
    /// List of file system entries.
    /// </summary>
    public IList<FileSystemEntry> Entries { get; set; } = null!;
}
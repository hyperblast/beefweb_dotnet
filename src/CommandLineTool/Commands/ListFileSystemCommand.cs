using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Commands;

[Command("list-fs", "lfs", Description = "List file system entries")]
public class ListFileSystemCommand(IClientProvider clientProvider, IConsole console, ITabularWriter writer)
    : ServerCommandBase(clientProvider)
{
    private static readonly bool[] LongFormatAlign = [true, true, true, false];

    [Argument(0, Description = "Path or special value 'roots'")]
    [Required]
    public string Path { get; set; } = null!;

    [Option("-p|--paths", Description = "Display full paths")]
    public bool FullPaths { get; set; }

    [Option("-l|--long", Description = "Display file sizes and timestamps")]
    public bool LongFormat { get; set; }

    [Option("-t|--type", Description = "Only display entries of specified type")]
    public string? Type { get; set; }

    public override async Task OnExecuteAsync(CancellationToken ct)
    {
        await base.OnExecuteAsync(ct);

        if (string.Equals(Path, "roots", StringComparison.OrdinalIgnoreCase))
        {
            var rootsResult = await Client.GetFileSystemRoots(ct);
            WriteEntries(rootsResult.Roots);
            return;
        }

        var entryType = Type != null
            ? ValueParser.ParseFileSystemEntryType(Type)
            : FileSystemEntryType.Unknown;

        var result = await Client.GetFileSystemEntries(Path, ct);
        var entries = result.Entries.AsEnumerable();

        if (Type != null)
        {
            entries = entries.Where(e => e.Type == entryType);
        }

        WriteEntries(entries);
    }

    private void WriteEntries(IEnumerable<FileSystemEntry> entries)
    {
        if (!LongFormat)
        {
            foreach (var entry in entries)
            {
                var path = FullPaths ? entry.Path : entry.Name;
                console.WriteLine(path);
            }

            return;
        }

        var output = entries
            .Select(e =>
                new[]
                {
                    e.Type == FileSystemEntryType.Directory ? "<DIR>" : e.Size.FormatFileSize(),
                    e.Timestamp.ToString("d"),
                    e.Timestamp.ToString("t"),
                    FullPaths ? e.Path : e.Name,
                })
            .ToList();

        writer.WriteTable(output, LongFormatAlign);
    }
}

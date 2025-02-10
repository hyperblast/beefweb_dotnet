using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client;

namespace Beefweb.CommandLineTool.Services;

public static class Extensions
{
    public static async ValueTask<int> GetPlaylistCount(this IPlayerClient client, CancellationToken ct)
    {
        return (await client.GetPlaylists(ct)).Count;
    }

    public static async ValueTask<PlaylistInfo> GetPlaylist(
        this IPlayerClient client, string playlistRef, bool zeroBasedIndexing, CancellationToken ct)
    {
        var playlists = await client.GetPlaylists(ct);
        return playlists.Get(playlistRef, zeroBasedIndexing);
    }

    public static PlaylistInfo Get(this IList<PlaylistInfo> playlists, string playlistRef, bool zeroBasedIndices)
    {
        if (string.Equals(playlistRef, Constants.CurrentPlaylist, StringComparison.OrdinalIgnoreCase))
        {
            return playlists.FirstOrDefault(p => p.IsCurrent) ?? playlists.First();
        }

        if (!IndexParser.TryParse(playlistRef, zeroBasedIndices, out var index))
        {
            if (!zeroBasedIndices && playlistRef == "0")
            {
                throw new InvalidRequestException("Playlist index (0) is out of range.");
            }

            return playlists.FirstOrDefault(p => p.Id == playlistRef) ??
                   throw new InvalidRequestException($"Unable to find playlist with id '{playlistRef}'.");
        }

        var realIndex = index.GetOffsetInclusive(playlists.Count);
        if (realIndex < 0 || realIndex >= playlists.Count)
        {
            throw new InvalidRequestException($"Playlist index ({playlistRef}) is out of range.");
        }

        return playlists[realIndex];
    }

    public static IEnumerable<int> GetItems(this Range range, int totalCount)
    {
        var (offset, length) = range.GetOffsetAndLengthInclusive(totalCount);
        return Enumerable.Range(offset, length);
    }

    public static PlaylistItemRange GetItemRange(this Range range, int totalCount)
    {
        var (offset, length) = range.GetOffsetAndLengthInclusive(totalCount);
        return new PlaylistItemRange(offset, length);
    }

    public static int GetOffsetInclusive(this Index index, int count)
    {
        return index.IsFromEnd ? checked(count - index.Value - 1) : index.Value;
    }

    private static (int offset, int length) GetOffsetAndLengthInclusive(this Range range, int count)
    {
        var start = range.Start.GetOffsetInclusive(count);
        var end = Math.Min(count - 1, range.End.GetOffsetInclusive(count));

        if (start < 0 || end < 0 || start > end || start >= count)
        {
            return (0, 0);
        }

        var length = end - start + 1;
        return (start, length);
    }

    public static IAsyncEnumerable<string> ReadLinesAsync(this TextReader reader) => ReadLinesImplAsync(reader);

    private static async IAsyncEnumerable<string> ReadLinesImplAsync(
        TextReader reader, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var line = await reader.ReadLineAsync(ct);
        while (line != null)
        {
            yield return line;
            line = await reader.ReadLineAsync(ct);
        }
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
            collection.Add(item);
    }

    public static async ValueTask AddRangeAsync<T>(
        this ICollection<T> collection, IAsyncEnumerable<T> items, CancellationToken ct = default)
    {
        await foreach (var item in items.WithCancellation(ct))
            collection.Add(item);
    }

    public static async ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items,
        CancellationToken ct = default)
    {
        var result = new List<T>();

        await foreach (var item in items.WithCancellation(ct))
            result.Add(item);

        return result;
    }

    public static IReadOnlyList<string> GetOrDefault(
        this IReadOnlyList<string>? values, IReadOnlyList<string> defaultValues)
    {
        return values is { Count: > 0 } ? values : defaultValues;
    }
}

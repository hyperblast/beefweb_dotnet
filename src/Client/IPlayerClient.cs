using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client
{
    public interface IPlayerClient : IDisposable
    {
        // Player API

        ValueTask<PlayerState> GetPlayerState(
            IReadOnlyList<string>? activeItemColumns = null, CancellationToken cancellationToken = default);
        
        ValueTask PlayCurrent(CancellationToken cancellationToken = default);

        ValueTask PlayRandom(CancellationToken cancellationToken = default);

        ValueTask PlayNext(CancellationToken cancellationToken = default);

        ValueTask PlayPrevious(CancellationToken cancellationToken = default);

        ValueTask Play(PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default);

        ValueTask Stop(CancellationToken cancellationToken = default);
        
        ValueTask Pause(CancellationToken cancellationToken = default);

        ValueTask TogglePause(CancellationToken cancellationToken = default);

        ValueTask UpdatePlayerState(UpdatePlayerStateRequest request, CancellationToken cancellationToken = default);

        ValueTask SetMuted(bool isMuted, CancellationToken cancellationToken = default);

        ValueTask SetMuted(BoolSwitch isMuted, CancellationToken cancellationToken = default);

        ValueTask SeekAbsolute(TimeSpan offset, CancellationToken cancellationToken = default);

        ValueTask SeekRelative(TimeSpan offset, CancellationToken cancellationToken = default);
        
        // Playlists API
        
        ValueTask<IList<PlaylistInfo>> GetPlaylists(CancellationToken cancellationToken = default);

        ValueTask<PlaylistItemsResult> GetPlaylistItems(
            PlaylistRef playlist,
            PlaylistItemRange range,
            IReadOnlyList<string> columns,
            CancellationToken cancellationToken = default);

        ValueTask SetCurrentPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default);

        ValueTask AddPlaylist(
            string? title = null,
            int? position = null,
            CancellationToken cancellationToken = default);

        ValueTask MovePlaylist(PlaylistRef playlist, int newPosition, CancellationToken cancellationToken = default);

        ValueTask RemovePlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default);

        ValueTask SetPlaylistTitle(
            PlaylistRef playlist, string newTitle, CancellationToken cancellationToken = default);

        ValueTask ClearPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default);

        ValueTask AddPlaylistItems(
            PlaylistRef playlist,
            IReadOnlyList<string> items,
            AddPlaylistItemsOptions? options = null,
            CancellationToken cancellationToken = default);

        ValueTask CopyPlaylistItems(
            PlaylistRef source,
            IReadOnlyList<int> itemIndices,
            PlaylistRef? target = null,
            int? targetIndex = null,
            CancellationToken cancellationToken = default);

        ValueTask MovePlaylistItems(
            PlaylistRef source,
            IReadOnlyList<int> itemIndices,
            PlaylistRef? target = null,
            int? targetIndex = null,
            CancellationToken cancellationToken = default);

        ValueTask RemovePlaylistItems(
            PlaylistRef playlist,
            IReadOnlyList<int> itemIndices,
            CancellationToken cancellationToken = default);

        ValueTask SortPlaylist(
            PlaylistRef playlist,
            string expression,
            bool descending = false,
            CancellationToken cancellationToken = default);

        ValueTask SortPlaylistRandomly(PlaylistRef playlist, CancellationToken cancellationToken = default);

        // File browser API

        ValueTask<FileSystemRootsResult> GetFileSystemRoots(CancellationToken cancellationToken = default);

        ValueTask<FileSystemEntriesResult> GetFileSystemEntries(
            string path, CancellationToken cancellationToken = default);

        // Artwork API

        ValueTask<IStreamedResult> GetArtwork(
            PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default);

        // Query API

        IPlayerQuery CreateQuery();
    }
}
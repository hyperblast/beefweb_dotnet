using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client;

/// <summary>
/// Player client contract.
/// </summary>
public interface IPlayerClient
{
    // Player API

    /// <summary>
    /// Get player state.
    /// </summary>
    /// <param name="activeItemColumns">Playlist item columns to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Player state.</returns>
    ValueTask<PlayerState> GetPlayerState(
        IReadOnlyList<string>? activeItemColumns = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays current item. Practically this method has effect of 'unpause'.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask PlayCurrent(CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays random item in the active playlist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask PlayRandom(CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays next item in the active playlist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask PlayNext(CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays previous item in the active playlist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask PlayPrevious(CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays next item in the active playlist,
    /// searching for an item which <paramref name="expression"/> value differs from the value of the active track.
    /// </summary>
    /// <param name="expression">Expression to search item by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask PlayNextBy(string expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays previous item in the active playlist,
    /// searching for an item which <paramref name="expression"/> value differs from the value of the active track.
    /// </summary>
    /// <param name="expression">Expression to search item by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask PlayPreviousBy(string expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays specified item.
    /// </summary>
    /// <param name="playlist">Playlist to play item from.</param>
    /// <param name="itemIndex">Item index in the specified playlist.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask Play(PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops playback.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask Stop(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses playback. Use <see cref="Play"/> to unpause.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask Pause(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles pause state.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask TogglePause(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates player state.
    /// </summary>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SetPlayerState(SetPlayerStateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets mute state.
    /// </summary>
    /// <param name="isMuted">New mute state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SetMuted(bool isMuted, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets or toggles mute state.
    /// </summary>
    /// <param name="isMuted">New mute state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SetMuted(BoolSwitch isMuted, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets player option.
    /// </summary>
    /// <param name="request">Set option request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SetOption(SetOptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets new playback position from the beginning of the track.
    /// </summary>
    /// <param name="offset">New playback position.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SeekAbsolute(TimeSpan offset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets new playback position relative to current playback position.
    /// </summary>
    /// <param name="offset">New playback position.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SeekRelative(TimeSpan offset, CancellationToken cancellationToken = default);

    // Playlists API

    /// <summary>
    /// Gets playlists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All available playlists.</returns>
    ValueTask<IList<PlaylistInfo>> GetPlaylists(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets playlist items.
    /// </summary>
    /// <param name="playlist">Playlist to get items from.</param>
    /// <param name="range">Playlist items range.</param>
    /// <param name="columns">Playlist item columns to return</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Playlist items information.</returns>
    ValueTask<PlaylistItemsResult> GetPlaylistItems(
        PlaylistRef playlist,
        PlaylistItemRange range,
        IReadOnlyList<string> columns,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets current playlist in the player UI.
    /// </summary>
    /// <param name="playlist">Playlist to select.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SetCurrentPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds new playlist.
    /// </summary>
    /// <param name="title">New playlist title.</param>
    /// <param name="position">New playlist position. By default created playlist is added to the end.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask AddPlaylist(
        string? title = null,
        int? position = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves playlist to new position.
    /// </summary>
    /// <param name="playlist">Playlist to move.</param>
    /// <param name="newPosition">New playlist position.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask MovePlaylist(PlaylistRef playlist, int newPosition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes playlist.
    /// </summary>
    /// <param name="playlist">Playlist to remove</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask RemovePlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets playlist title.
    /// </summary>
    /// <param name="playlist">Playlist to change.</param>
    /// <param name="newTitle">New playlist title.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SetPlaylistTitle(
        PlaylistRef playlist, string newTitle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears (removes all playlist items) specified playlist
    /// </summary>
    /// <param name="playlist">Playlist to clear.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask ClearPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds playlist items to the specified playlist.
    /// </summary>
    /// <param name="playlist">Playlist to change.</param>
    /// <param name="items">Items (paths, URLs, etc) to add.</param>
    /// <param name="options">Additional request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask AddPlaylistItems(
        PlaylistRef playlist,
        IReadOnlyList<string> items,
        AddPlaylistItemsOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies playlist items.
    /// </summary>
    /// <param name="source">Source playlist.</param>
    /// <param name="itemIndices">Indices of items to copy.</param>
    /// <param name="target">Target playlist, if not specified target is the same as <paramref name="source"/>.</param>
    /// <param name="targetIndex">Target index to copy items to, if not specified items are copied to the end of the playlist.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask CopyPlaylistItems(
        PlaylistRef source,
        IReadOnlyList<int> itemIndices,
        PlaylistRef? target = null,
        int? targetIndex = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves playlist items.
    /// </summary>
    /// <param name="source">Source playlist.</param>
    /// <param name="itemIndices">Indices of items to move.</param>
    /// <param name="target">Target playlist, if not specified target is the same as <paramref name="source"/>.</param>
    /// <param name="targetIndex">Target index to move items to, if not specified items are moved to the end of the playlist.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask MovePlaylistItems(
        PlaylistRef source,
        IReadOnlyList<int> itemIndices,
        PlaylistRef? target = null,
        int? targetIndex = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes specified playlist items.
    /// </summary>
    /// <param name="playlist">Playlist to change.</param>
    /// <param name="itemIndices">Indices of items to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask RemovePlaylistItems(
        PlaylistRef playlist,
        IReadOnlyList<int> itemIndices,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sorts items in the playlist.
    /// </summary>
    /// <param name="playlist">Playlist to change.</param>
    /// <param name="expression">Sort expression, e.g. %title%</param>
    /// <param name="descending">If true items are sorted in the descending order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SortPlaylist(
        PlaylistRef playlist,
        string expression,
        bool descending = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Randomly rearranges items in the playlist.
    /// </summary>
    /// <param name="playlist">Playlist to change.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Request task.</returns>
    ValueTask SortPlaylistRandomly(PlaylistRef playlist, CancellationToken cancellationToken = default);

    // File browser API

    /// <summary>
    /// Gets available file system roots.
    /// These are directories configured by the user to be browsable via API.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File system roots.</returns>
    ValueTask<FileSystemRootsResult> GetFileSystemRoots(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file system entries for the specified directory.
    /// This directory must be a subdirectory of the configured file system roots.
    /// </summary>
    /// <param name="path">Directory path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File system entries.</returns>
    ValueTask<FileSystemEntriesResult> GetFileSystemEntries(
        string path, CancellationToken cancellationToken = default);

    // Artwork API

    /// <summary>
    /// Gets album art image for currently playing item.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Artwork image stream, or null, if no artwork is found or no item is playing currently.</returns>
    ValueTask<IStreamedResult?> GetCurrentArtwork(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets album art image for the specified playlist item.
    /// </summary>
    /// <param name="playlist">Playlist to use.</param>
    /// <param name="itemIndex">Playlist item index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Artwork image stream, or null, if no artwork is found for specified item.</returns>
    ValueTask<IStreamedResult?> GetArtwork(
        PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default);

    // Query API

    /// <summary>
    /// Creates new player query.
    /// </summary>
    /// <returns>Created query.</returns>
    IPlayerQuery CreateQuery();
}

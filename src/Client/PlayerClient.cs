using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client.Infrastructure;

namespace Beefweb.Client;

/// <summary>
/// Player client.
/// </summary>
public sealed class PlayerClient : IPlayerClient
{
    private readonly IRequestHandler _handler;
    private IDisposable? _lifetime;

    /// <summary>
    /// Creates new instance for specified <paramref name="baseUri"/>.
    /// </summary>
    /// <param name="baseUri">Base uri. This value should not include '/api' in path</param>
    public PlayerClient(Uri baseUri)
        : this(baseUri, new HttpClient())
    {
    }

    /// <summary>
    /// Creates new instance for specified <paramref name="baseUri"/> and <paramref name="client"/>.
    /// </summary>
    /// <param name="baseUri">Base uri. This value should not include '/api' in path.</param>
    /// <param name="client"><see cref="HttpClient"/> to use.</param>
    /// <param name="disposeClient">If true <paramref name="client"/> will be disposed with this instance.</param>
    public PlayerClient(Uri baseUri, HttpClient client, bool disposeClient = true)
        : this(
            new RequestHandler(baseUri, client, new LineReaderFactory(new GrowableBufferFactory())),
            disposeClient ? client : null)
    {
    }

    internal PlayerClient(IRequestHandler handler, IDisposable? lifetime = null)
    {
        _handler = handler;
        _lifetime = lifetime;
    }

    // Player API

    /// <inheritdoc />
    public async ValueTask<PlayerState> GetPlayerState(
        IReadOnlyList<string>? activeItemColumns = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = activeItemColumns?.Count > 0
            ? new QueryParameterCollection { ["columns"] = activeItemColumns }
            : null;
        var result = await _handler.Get<PlayerQueryResult>("api/player", queryParams, cancellationToken);
        return result.Player!;
    }

    /// <inheritdoc />
    public async ValueTask PlayCurrent(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/play", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask PlayRandom(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/play/random", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask PlayNext(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/next", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask PlayPrevious(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/previous", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask PlayNextBy(string expression, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/next", new { by = expression }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask PlayPreviousBy(string expression, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/previous", new { by = expression }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask Play(PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default)
    {
        await _handler.Post(
            FormattableString.Invariant($"api/player/play/{playlist}/{itemIndex}"), null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask Stop(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/stop", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask Pause(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/pause", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask TogglePause(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/pause/toggle", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SetPlayerState(
        SetPlayerStateRequest request, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player", request, cancellationToken);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="isMuted"></param>
    /// <param name="cancellationToken"></param>
    public async ValueTask SetMuted(bool isMuted, CancellationToken cancellationToken = default)
    {
        await SetMuted(isMuted ? BoolSwitch.True : BoolSwitch.False, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SetMuted(BoolSwitch isMuted, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { IsMuted = isMuted }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SetOption(SetOptionRequest request, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { Options = new[] { request } }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SeekAbsolute(TimeSpan offset, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { Position = offset }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SeekRelative(TimeSpan offset, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { RelativePosition = offset }, cancellationToken);
    }

    // Playlists API

    /// <inheritdoc />
    public async ValueTask<IList<PlaylistInfo>> GetPlaylists(CancellationToken cancellationToken = default)
    {
        var result = await _handler.Get<PlayerQueryResult>("api/playlists", null, cancellationToken);
        return result.Playlists!;
    }

    /// <inheritdoc />
    public async ValueTask<PlaylistItemsResult> GetPlaylistItems(
        PlaylistRef playlist,
        PlaylistItemRange range,
        IReadOnlyList<string> columns,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new QueryParameterCollection { ["columns"] = columns };
        var result = await _handler.Get<PlayerQueryResult>(
            $"api/playlists/{playlist}/items/{range}", queryParams, cancellationToken);

        return result.PlaylistItems!;
    }

    /// <inheritdoc />
    public async ValueTask SetCurrentPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/playlists", new { current = playlist.ToString() }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask AddPlaylist(
        string? title = null, int? position = null, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/playlists/add", new { title, index = position }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask MovePlaylist(
        PlaylistRef playlist, int newPosition, CancellationToken cancellationToken = default)
    {
        await _handler.Post(
            FormattableString.Invariant($"api/playlists/move/{playlist}/{newPosition}"), null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask RemovePlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler.Post($"api/playlists/remove/{playlist}", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SetPlaylistTitle(
        PlaylistRef playlist, string newTitle, CancellationToken cancellationToken = default)
    {
        await _handler.Post($"api/playlists/{playlist}", new { title = newTitle }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask ClearPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler.Post($"api/playlists/{playlist}/clear", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask AddPlaylistItems(
        PlaylistRef playlist,
        IReadOnlyList<string> items,
        AddPlaylistItemsOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        static bool? NormalizeBool(bool? value) => value == true ? value : null;

        var body = new
        {
            items,
            index = options?.TargetPosition,
            async = NormalizeBool(options?.ProcessAsynchronously),
            play = NormalizeBool(options?.PlayAddedItems),
            replace = NormalizeBool(options?.ReplaceExistingItems),
        };

        await _handler.Post($"api/playlists/{playlist}/items/add", body, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask CopyPlaylistItems(
        PlaylistRef source,
        IReadOnlyList<int> itemIndices,
        PlaylistRef? target = null,
        int? targetIndex = null,
        CancellationToken cancellationToken = default)
    {
        var url = target == null || source == target
            ? $"api/playlists/{source}/items/copy"
            : $"api/playlists/{source}/{target}/items/copy";

        await _handler.Post(url, new { items = itemIndices, targetIndex }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask MovePlaylistItems(
        PlaylistRef source,
        IReadOnlyList<int> itemIndices,
        PlaylistRef? target = null,
        int? targetIndex = null,
        CancellationToken cancellationToken = default)
    {
        var url = target == null || source == target
            ? $"api/playlists/{source}/items/move"
            : $"api/playlists/{source}/{target}/items/move";

        await _handler.Post(url, new { items = itemIndices, targetIndex }, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask RemovePlaylistItems(PlaylistRef playlist, IReadOnlyList<int> itemIndices,
        CancellationToken cancellationToken = default)
    {
        await _handler.Post(
            $"api/playlists/{playlist}/items/remove",
            new { items = itemIndices },
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SortPlaylist(PlaylistRef playlist, string expression, bool descending = false,
        CancellationToken cancellationToken = default)
    {
        await _handler.Post(
            $"api/playlists/{playlist}/items/sort",
            new { by = expression, desc = descending },
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask SortPlaylistRandomly(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler.Post($"api/playlists/{playlist}/sort", new { random = true }, cancellationToken);
    }

    // File browser API

    /// <inheritdoc />
    public async ValueTask<FileSystemRootsResult> GetFileSystemRoots(CancellationToken cancellationToken = default)
    {
        return await _handler.Get<FileSystemRootsResult>("api/browser/roots", null, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<FileSystemEntriesResult> GetFileSystemEntries(string path,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new QueryParameterCollection { ["path"] = path };
        return await _handler.Get<FileSystemEntriesResult>("api/browser/entries", queryParams, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<IStreamedResult> GetArtwork(
        PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default)
    {
        return await _handler.GetStream(
            FormattableString.Invariant($"api/artwork/{playlist}/{itemIndex}"), null, cancellationToken);
    }

    // Query API

    /// <inheritdoc />
    public IPlayerQuery CreateQuery() => new PlayerQuery(_handler);

    /// <inheritdoc />
    public void Dispose() => Interlocked.Exchange(ref _lifetime, null)?.Dispose();
}

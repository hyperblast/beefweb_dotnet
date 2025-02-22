using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client.Infrastructure;
using static System.FormattableString;
using static Beefweb.Client.Infrastructure.ArgumentValidator;

namespace Beefweb.Client;

/// <summary>
/// Player client.
/// </summary>
public sealed class PlayerClient : IPlayerClient, IDisposable
{
    private readonly IRequestHandler _handler;
    private IDisposable? _lifetime;
    private readonly JsonSerializerOptions _clientConfigOptions;

    /// <summary>
    /// Creates new instance for specified <paramref name="baseUri"/>.
    /// </summary>
    /// <param name="baseUri">Base uri. This value should not include '/api' in path</param>
    /// <param name="clientConfigOptions">Options for serializing client configuration.</param>
    public PlayerClient(Uri baseUri, JsonSerializerOptions? clientConfigOptions = null)
        : this(baseUri, new HttpClient(), disposeClient: true, clientConfigOptions)
    {
    }

    /// <summary>
    /// Creates new instance for specified <paramref name="baseUri"/> and <paramref name="client"/>.
    /// </summary>
    /// <param name="baseUri">Base uri. This value should not include '/api' in path.</param>
    /// <param name="client"><see cref="HttpClient"/> to use.</param>
    /// <param name="disposeClient">If true <paramref name="client"/> will be disposed with this instance.</param>
    /// <param name="clientConfigOptions">Options for serializing client configuration.</param>
    public PlayerClient(
        Uri baseUri, HttpClient client, bool disposeClient = true, JsonSerializerOptions? clientConfigOptions = null)
        : this(
            new RequestHandler(baseUri, client, new LineReaderFactory(new GrowableBufferFactory())),
            disposeClient ? client : null)
    {
    }

    internal PlayerClient(
        IRequestHandler handler, IDisposable? lifetime = null, JsonSerializerOptions? clientConfigOptions = null)
    {
        _handler = handler;
        _lifetime = lifetime;
        _clientConfigOptions = clientConfigOptions ?? JsonSerializerOptions.Default;
    }

    // Player API

    /// <inheritdoc />
    public async ValueTask<PlayerState> GetPlayerState(
        IReadOnlyList<string>? activeItemColumns = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = activeItemColumns is { Count: > 0 }
            ? new QueryParameterCollection { ["columns"] = activeItemColumns }
            : null;

        var result = await _handler
            .Get<PlayerQueryResult>("api/player", queryParams, cancellationToken)
            .ConfigureAwait(false);

        return result.Player ?? throw PropertyIsNull("player");
    }

    /// <inheritdoc />
    public async ValueTask PlayCurrent(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/play", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayRandom(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/play/random", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayNext(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/next", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayPrevious(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/previous", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayNextBy(string expression, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/next", new { by = expression }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayPreviousBy(string expression, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/previous", new { by = expression }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask Play(PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default)
    {
        await _handler
            .Post(Invariant($"api/player/play/{playlist}/{itemIndex}"), null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask Stop(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/stop", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask Pause(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/pause", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask TogglePause(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/pause/toggle", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayOrPause(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player/play-pause", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetPlayerState(
        SetPlayerStateRequest request, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/player", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="isMuted"></param>
    /// <param name="cancellationToken"></param>
    public async ValueTask SetMuted(bool isMuted, CancellationToken cancellationToken = default)
    {
        await SetMuted(isMuted ? BoolSwitch.True : BoolSwitch.False, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetMuted(BoolSwitch isMuted, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { IsMuted = isMuted }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetVolume(double volume, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { Volume = volume }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask VolumeStep(int direction, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { VolumeStep = direction }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetOption(SetOptionRequest request, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { Options = [request] }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SeekAbsolute(TimeSpan offset, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { Position = offset }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SeekRelative(TimeSpan offset, CancellationToken cancellationToken = default)
    {
        await SetPlayerState(new SetPlayerStateRequest { RelativePosition = offset }, cancellationToken)
            .ConfigureAwait(false);
    }

    // Playback queue API

    /// <inheritdoc />
    public async ValueTask<IList<PlayQueueItemInfo>> GetPlayQueue(
        IReadOnlyList<string>? columns = null, CancellationToken cancellationToken = default)
    {
        var queryParams = columns is { Count: > 0 }
            ? new QueryParameterCollection { ["columns"] = columns }
            : null;

        var result = await _handler
            .Get<PlayerQueryResult>("api/playqueue", queryParams, cancellationToken)
            .ConfigureAwait(false);

        return result.PlayQueue ?? throw PropertyIsNull("playlistItems");
    }

    /// <inheritdoc />
    public async ValueTask AddToPlayQueue(PlaylistRef playlist, int itemIndex, int? queueIndex = null,
        CancellationToken cancellationToken = default)
    {
        var body = queueIndex != null
            ? new { plref = playlist, itemIndex, queueIndex }
            : (object) new { plref = playlist, itemIndex };

        await _handler.Post("api/playqueue/add", body, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask RemoveFromPlayQueue(int queueIndex, CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/playqueue/remove", new { queueIndex }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask RemoveFromPlayQueue(
        PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default)
    {
        await _handler
            .Post("api/playqueue/remove", new { plref = playlist, itemIndex }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ClearPlayQueue(CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/playqueue/clear", null, cancellationToken).ConfigureAwait(false);
    }

    // Playlists API

    /// <inheritdoc />
    public async ValueTask<PlaylistInfo> GetPlaylist(
        PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        return await _handler
            .Get<PlaylistInfo>($"api/playlists/{playlist}", null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<IList<PlaylistInfo>> GetPlaylists(CancellationToken cancellationToken = default)
    {
        var result = await _handler
            .Get<PlayerQueryResult>("api/playlists", null, cancellationToken)
            .ConfigureAwait(false);

        return result.Playlists ?? throw PropertyIsNull("playlists");
    }

    /// <inheritdoc />
    public async ValueTask<PlaylistItemsResult> GetPlaylistItems(
        PlaylistRef playlist,
        PlaylistItemRange range,
        IReadOnlyList<string> columns,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new QueryParameterCollection { ["columns"] = columns };
        var result = await _handler
            .Get<PlayerQueryResult>($"api/playlists/{playlist}/items/{range}", queryParams, cancellationToken)
            .ConfigureAwait(false);

        return result.PlaylistItems ?? throw PropertyIsNull("playlistItems");
    }

    /// <inheritdoc />
    public async ValueTask SetCurrentPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler
            .Post("api/playlists", new { current = playlist }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<PlaylistInfo> AddPlaylist(
        string? title = null, int? position = null, bool setCurrent = false,
        CancellationToken cancellationToken = default)
    {
        return await _handler
            .Post<PlaylistInfo>("api/playlists/add", new { title, index = position, setCurrent }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask MovePlaylist(
        PlaylistRef playlist, int newPosition, CancellationToken cancellationToken = default)
    {
        await _handler
            .Post(Invariant($"api/playlists/move/{playlist}/{newPosition}"), null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask RemovePlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler.Post($"api/playlists/remove/{playlist}", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetPlaylistTitle(
        PlaylistRef playlist, string newTitle, CancellationToken cancellationToken = default)
    {
        await _handler
            .Post($"api/playlists/{playlist}", new { title = newTitle }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ClearPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler.Post($"api/playlists/{playlist}/clear", null, cancellationToken).ConfigureAwait(false);
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

        await _handler.Post($"api/playlists/{playlist}/items/add", body, cancellationToken).ConfigureAwait(false);
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

        await _handler.Post(url, new { items = itemIndices, targetIndex }, cancellationToken).ConfigureAwait(false);
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

        await _handler.Post(url, new { items = itemIndices, targetIndex }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask RemovePlaylistItems(PlaylistRef playlist, IReadOnlyList<int> itemIndices,
        CancellationToken cancellationToken = default)
    {
        await _handler
            .Post($"api/playlists/{playlist}/items/remove", new { items = itemIndices }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SortPlaylist(PlaylistRef playlist, string expression, bool descending = false,
        CancellationToken cancellationToken = default)
    {
        await _handler
            .Post($"api/playlists/{playlist}/items/sort", new { by = expression, desc = descending }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SortPlaylistRandomly(PlaylistRef playlist, CancellationToken cancellationToken = default)
    {
        await _handler
            .Post($"api/playlists/{playlist}/items/sort", new { random = true }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<OutputsInfo> GetOutputs(CancellationToken cancellationToken = default)
    {
        var result = await _handler.Get<PlayerQueryResult>("api/outputs", null, cancellationToken).ConfigureAwait(false);
        return result.Outputs ?? throw PropertyIsNull("outputs");
    }

    /// <inheritdoc />
    public async ValueTask SetOutputDevice(string? typeId, string deviceId,
        CancellationToken cancellationToken = default)
    {
        await _handler.Post("api/outputs/active", new { typeId, deviceId }, cancellationToken).ConfigureAwait(false);
    }

    // File browser API

    /// <inheritdoc />
    public async ValueTask<FileSystemRootsResult> GetFileSystemRoots(CancellationToken cancellationToken = default)
    {
        return await _handler
            .Get<FileSystemRootsResult>("api/browser/roots", null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<FileSystemEntriesResult> GetFileSystemEntries(string path,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new QueryParameterCollection { ["path"] = path };
        return await _handler
            .Get<FileSystemEntriesResult>("api/browser/entries", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<IStreamedResult?> GetCurrentArtwork(CancellationToken cancellationToken = default)
    {
        return await _handler.GetStream("api/artwork/current", null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<IStreamedResult?> GetArtwork(
        PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default)
    {
        return await _handler
            .GetStream(Invariant($"api/artwork/{playlist}/{itemIndex}"), null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<string?> GetClientConfig(string id, CancellationToken cancellationToken = default)
    {
        var result = await this.GetClientConfig<RawJson?>(id, cancellationToken).ConfigureAwait(false);
        return result?.Value == "null" ? null : result?.Value;
    }

    /// <inheritdoc />
    public async ValueTask<object?> GetClientConfig(
        string id, Type configType, CancellationToken cancellationToken = default)
    {
        ValidateConfigId(id);

        return await _handler
            .Get(configType,
                $"api/clientconfig/{id}",
                serializerOptions: _clientConfigOptions,
                allowNullResponse: true,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetClientConfig(string id, string value, CancellationToken cancellationToken = default)
    {
        ValidateConfigId(id);

        await SetClientConfig(id, new RawJson(value), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SetClientConfig(string id, object value, CancellationToken cancellationToken = default)
    {
        ValidateConfigId(id);

        await _handler
            .Post(
                null,
                $"api/clientconfig/{id}",
                value,
                serializerOptions: _clientConfigOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask RemoveClientConfig(string id, CancellationToken cancellationToken = default)
    {
        await _handler.Post($"api/clientconfig/remove/{id}", null, cancellationToken).ConfigureAwait(false);
    }

    // Query API

    /// <inheritdoc />
    public IPlayerQuery CreateQuery() => new PlayerQuery(_handler);

    /// <inheritdoc />
    public void Dispose() => Interlocked.Exchange(ref _lifetime, null)?.Dispose();

    private static PlayerClientException PropertyIsNull(string name)
    {
        return new PlayerClientException(
            $"Expected response property '{name}' to be not null.",
            HttpRequestError.InvalidResponse,
            HttpStatusCode.OK);
    }
}

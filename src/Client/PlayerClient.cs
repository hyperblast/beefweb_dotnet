using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client.Infrastructure;

namespace Beefweb.Client
{
    public sealed class PlayerClient : IPlayerClient
    {
        private readonly IRequestHandler _handler;
        private IDisposable? _lifetime;

        public PlayerClient(Uri baseUri)
            : this(baseUri, new HttpClient())
        {
        }

        public PlayerClient(Uri baseUri, HttpClient client, bool disposeClient = true)
            : this(
                new RequestHandler(baseUri, client, new LineReaderFactory(new GrowableBufferFactory())),
                disposeClient ? client : null)
        {
        }

        public PlayerClient(IRequestHandler handler, IDisposable? lifetime = null)
        {
            _handler = handler;
            _lifetime = lifetime;
        }

        // Player API

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

        public async ValueTask PlayCurrent(CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player/play", null, cancellationToken);
        }

        public async ValueTask PlayRandom(CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player/play/random", null, cancellationToken);
        }

        public async ValueTask PlayNext(CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player/next", null, cancellationToken);
        }

        public async ValueTask PlayPrevious(CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player/previous", null, cancellationToken);
        }

        public async ValueTask Play(PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default)
        {
            await _handler.Post(
                FormattableString.Invariant($"api/player/play/{playlist}/{itemIndex}"), null, cancellationToken);
        }

        public async ValueTask Stop(CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player/stop", null, cancellationToken);
        }

        public async ValueTask Pause(CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player/pause", null, cancellationToken);
        }

        public async ValueTask TogglePause(CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player/pause/toggle", null, cancellationToken);
        }

        public async ValueTask UpdatePlayerState(
            UpdatePlayerStateRequest request, CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/player", request, cancellationToken);
        }

        public async ValueTask SetMuted(bool isMuted, CancellationToken cancellationToken = default)
        {
            await SetMuted(isMuted ? BoolSwitch.True : BoolSwitch.False, cancellationToken);
        }

        public async ValueTask SetMuted(BoolSwitch isMuted, CancellationToken cancellationToken = default)
        {
            await UpdatePlayerState(new UpdatePlayerStateRequest { IsMuted = isMuted }, cancellationToken);
        }

        public async ValueTask SeekAbsolute(TimeSpan offset, CancellationToken cancellationToken = default)
        {
            await UpdatePlayerState(new UpdatePlayerStateRequest { Position = offset }, cancellationToken);
        }

        public async ValueTask SeekRelative(TimeSpan offset, CancellationToken cancellationToken = default)
        {
            await UpdatePlayerState(new UpdatePlayerStateRequest { RelativePosition = offset }, cancellationToken);
        }

        // Playlists API

        public async ValueTask<IList<PlaylistInfo>> GetPlaylists(CancellationToken cancellationToken = default)
        {
            var result = await _handler.Get<PlayerQueryResult>("api/playlists", null, cancellationToken);
            return result.Playlists!;
        }

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

        public async ValueTask SetCurrentPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/playlists", new { current = playlist.ToString() }, cancellationToken);
        }

        public async ValueTask AddPlaylist(
            string? title = null, int? position = null, CancellationToken cancellationToken = default)
        {
            await _handler.Post("api/playlists/add", new { title, index = position }, cancellationToken);
        }

        public async ValueTask MovePlaylist(
            PlaylistRef playlist, int newPosition, CancellationToken cancellationToken = default)
        {
            await _handler.Post(
                FormattableString.Invariant($"api/playlists/move/{playlist}/{newPosition}"), null, cancellationToken);
        }

        public async ValueTask RemovePlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
        {
            await _handler.Post($"api/playlists/remove/{playlist}", null, cancellationToken);
        }

        public async ValueTask SetPlaylistTitle(
            PlaylistRef playlist, string newTitle, CancellationToken cancellationToken = default)
        {
            await _handler.Post($"api/playlists/{playlist}", new { title = newTitle }, cancellationToken);
        }

        public async ValueTask ClearPlaylist(PlaylistRef playlist, CancellationToken cancellationToken = default)
        {
            await _handler.Post($"api/playlists/{playlist}/clear", null, cancellationToken);
        }

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

        public async ValueTask RemovePlaylistItems(PlaylistRef playlist, IReadOnlyList<int> itemIndices,
            CancellationToken cancellationToken = default)
        {
            await _handler.Post(
                $"api/playlists/{playlist}/items/remove",
                new { items = itemIndices },
                cancellationToken);
        }

        public async ValueTask SortPlaylist(PlaylistRef playlist, string expression, bool descending = false,
            CancellationToken cancellationToken = default)
        {
            await _handler.Post(
                $"api/playlists/{playlist}/items/sort",
                new { by = expression, desc = descending },
                cancellationToken);
        }

        public async ValueTask SortPlaylistRandomly(PlaylistRef playlist, CancellationToken cancellationToken = default)
        {
            await _handler.Post($"api/playlists/{playlist}/sort", new { random = true }, cancellationToken);
        }

        // File browser API

        public async ValueTask<FileSystemRootsResult> GetFileSystemRoots(CancellationToken cancellationToken = default)
        {
            return await _handler.Get<FileSystemRootsResult>("api/browser/roots", null, cancellationToken);
        }

        public async ValueTask<FileSystemEntriesResult> GetFileSystemEntries(string path,
            CancellationToken cancellationToken = default)
        {
            var queryParams = new QueryParameterCollection { ["path"] = path };
            return await _handler.Get<FileSystemEntriesResult>("api/browser/entries", queryParams, cancellationToken);
        }

        public async ValueTask<IStreamedResult> GetArtwork(
            PlaylistRef playlist, int itemIndex, CancellationToken cancellationToken = default)
        {
            return await _handler.GetStream(
                FormattableString.Invariant($"api/artwork/{playlist}/{itemIndex}"), null, cancellationToken);
        }

        // Query API

        public IPlayerQuery CreateQuery() => new PlayerQuery(_handler);

        public void Dispose() => Interlocked.Exchange(ref _lifetime, null)?.Dispose();
    }
}
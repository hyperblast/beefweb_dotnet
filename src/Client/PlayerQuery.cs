using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client.Infrastructure;

namespace Beefweb.Client
{
    public sealed class PlayerQuery : IPlayerQuery
    {
        private readonly IRequestHandler _handler;

        private bool _includePlayer;
        private bool _includePlaylists;
        private bool _includePlaylistItems;
        private IReadOnlyList<string>? _activeItemColumns;
        private IReadOnlyList<string>? _playlistItemColumns;
        private PlaylistRef _playlist;
        private PlaylistItemRange _playlistItemRange;

        public PlayerQuery(IRequestHandler handler)
        {
            _handler = handler;
        }

        public IPlayerQuery IncludePlayer(IReadOnlyList<string>? activeItemColumns = null)
        {
            _includePlayer = true;
            _activeItemColumns = activeItemColumns;
            return this;
        }

        public IPlayerQuery IncludePlaylists()
        {
            _includePlaylists = true;
            return this;
        }

        public IPlayerQuery IncludePlaylistItems(
            PlaylistRef playlist, PlaylistItemRange itemRange, IReadOnlyList<string>? itemColumns = null)
        {
            _includePlaylistItems = true;
            _playlist = playlist;
            _playlistItemRange = itemRange;
            _playlistItemColumns = itemColumns;
            return this;
        }

        public ValueTask<PlayerQueryResult> Execute(CancellationToken cancellationToken = default)
        {
            return _handler.Get<PlayerQueryResult>("api/query", BuildQuery(), cancellationToken);
        }

        public IAsyncEnumerable<PlayerEvent> ReadEvents(CancellationToken cancellationToken)
        {
            return _handler.GetEvents<PlayerEvent>("api/query/events", BuildQuery(), cancellationToken);
        }

        public IAsyncEnumerable<PlayerQueryResult> ReadUpdates(CancellationToken cancellationToken = default)
        {
            return _handler.GetEvents<PlayerQueryResult>("api/query/updates", BuildQuery(), cancellationToken);
        }

        private QueryParameterCollection BuildQuery()
        {
            var query = new QueryParameterCollection();

            if (_includePlayer)
            {
                query["player"] = true;

                if (_activeItemColumns?.Count > 0)
                    query["trcolumns"] = _activeItemColumns;
            }

            if (_includePlaylists)
            {
                query["playlists"] = true;
            }

            if (_includePlaylistItems)
            {
                query["playlistItems"] = true;
                query["plref"] = _playlist;
                query["plrange"] = _playlistItemRange;

                if (_playlistItemColumns?.Count > 0)
                    query["plcolumns"] = _playlistItemColumns;
            }

            if (query.Count == 0)
            {
                throw new InvalidOperationException(
                    "Query is empty, call at least one IncludeXxx() method before executing the query.");
            }

            return query;
        }
    }
}
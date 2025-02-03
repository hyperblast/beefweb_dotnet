using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.Client.Infrastructure;

namespace Beefweb.Client;

internal sealed class PlayerQuery : IPlayerQuery
{
    private readonly IRequestHandler _handler;

    private bool _includePlayer;
    private bool _includePlayQueue;
    private bool _includePlaylists;
    private bool _includePlaylistItems;
    private bool _hasPlaylistItemsParameters;
    private IReadOnlyList<string>? _activeItemColumns;
    private IReadOnlyList<string>? _playQueueColumns;
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

    public IPlayerQuery IncludePlaylistItems()
    {
        _includePlaylistItems = true;
        return this;
    }

    public IPlayerQuery IncludePlaylistItems(
        PlaylistRef playlist, PlaylistItemRange itemRange, IReadOnlyList<string> itemColumns)
    {
        _includePlaylistItems = true;
        _playlist = playlist;
        _playlistItemRange = itemRange;
        _playlistItemColumns = itemColumns;
        _hasPlaylistItemsParameters = true;
        return this;
    }

    public IPlayerQuery IncludePlayQueue(IReadOnlyList<string>? columns = null)
    {
        _includePlayQueue = true;
        _playQueueColumns = columns;
        return this;
    }

    public async ValueTask<PlayerQueryResult> Execute(CancellationToken cancellationToken = default)
    {
        return await _handler
            .Get<PlayerQueryResult>("api/query", BuildQuery(true), cancellationToken)
            .ConfigureAwait(false);
    }

    public IAsyncEnumerable<PlayerEvent> ReadEvents()
    {
        return _handler.GetEvents<PlayerEvent>("api/query/events", BuildQuery(false));
    }

    public IAsyncEnumerable<PlayerQueryResult> ReadUpdates()
    {
        return _handler.GetEvents<PlayerQueryResult>("api/query/updates", BuildQuery(true));
    }

    private QueryParameterCollection BuildQuery(bool wantPlaylistItemsParameters)
    {
        var query = new QueryParameterCollection();

        if (_includePlayer)
        {
            query["player"] = true;

            if (_activeItemColumns is { Count: > 0 })
                query["trcolumns"] = _activeItemColumns;
        }

        if (_includePlayQueue)
        {
            query["playQueue"] = true;

            if (_playQueueColumns is { Count: > 0 })
                query["qcolumns"] = _playQueueColumns;
        }

        if (_includePlaylists)
        {
            query["playlists"] = true;
        }

        if (_includePlaylistItems)
        {
            query["playlistItems"] = true;

            if (wantPlaylistItemsParameters)
            {
                if (!_hasPlaylistItemsParameters)
                {
                    throw new InvalidOperationException(
                        "Use IncludePlaylists() overload with playlist, range and columns to call this method.");
                }

                query["plref"] = _playlist;
                query["plrange"] = _playlistItemRange;
                query["plcolumns"] = _playlistItemColumns;
            }
        }

        if (query.Count == 0)
        {
            throw new InvalidOperationException(
                "Query is empty, call at least one IncludeXxx() method before calling this method.");
        }

        return query;
    }
}

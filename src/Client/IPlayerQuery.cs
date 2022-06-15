using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client
{
    public interface IPlayerQuery
    {
        IPlayerQuery IncludePlayer(IReadOnlyList<string>? activeItemColumns = null);

        IPlayerQuery IncludePlaylists();

        IPlayerQuery IncludePlaylistItems(
            PlaylistRef playlist, PlaylistItemRange itemRange, IReadOnlyList<string>? itemColumns = null);

        ValueTask<PlayerQueryResult> Execute(CancellationToken cancellationToken = default);

        IAsyncEnumerable<PlayerQueryResult> ReadUpdates(CancellationToken cancellationToken = default);

        IAsyncEnumerable<PlayerEvent> ReadEvents(CancellationToken cancellationToken = default);
    }
}
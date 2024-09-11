using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client;

/// <summary>
/// Player query contract.
/// </summary>
public interface IPlayerQuery
{
    /// <summary>
    /// Configures query to include player state information.
    /// </summary>
    /// <param name="activeItemColumns">List of columns of playlist item being played to return.</param>
    /// <returns>Configured query.</returns>
    IPlayerQuery IncludePlayer(IReadOnlyList<string>? activeItemColumns = null);

    /// <summary>
    /// Configures query to include playlists information.
    /// </summary>
    /// <returns>Configured query.</returns>
    IPlayerQuery IncludePlaylists();

    /// <summary>
    /// Configures player to include playlist items information
    /// </summary>
    /// <param name="playlist">Playlist to watch.</param>
    /// <param name="itemRange">Playlist items to watch.</param>
    /// <param name="itemColumns">Playlist item columns to return.</param>
    /// <returns>Configured query.</returns>
    IPlayerQuery IncludePlaylistItems(
        PlaylistRef playlist, PlaylistItemRange itemRange, IReadOnlyList<string> itemColumns);

    /// <summary>
    /// Executes this query and returns player information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Player information for this query.</returns>
    ValueTask<PlayerQueryResult> Execute(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads updates of player information specified by this query.
    /// </summary>
    /// <returns>Player updates stream.</returns>
    IAsyncEnumerable<PlayerQueryResult> ReadUpdates();

    /// <summary>
    /// Reads player events specified by this query.
    /// </summary>
    /// <returns>Player events stream.</returns>
    IAsyncEnumerable<PlayerEvent> ReadEvents();
}

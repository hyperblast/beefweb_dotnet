using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client;

/// <summary>
/// Extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets client configuration as value of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="client">Client to use.</param>
    /// <param name="id">Configuration id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="T">Configuration type.</typeparam>
    /// <returns>Request task.</returns>
    public static async ValueTask<T?> GetClientConfig<T>(
        this IPlayerClient client, string id, CancellationToken cancellationToken = default)
    {
        return (T?)await client
            .GetClientConfig(id, typeof(T), cancellationToken)
            .ConfigureAwait(false);
    }
}

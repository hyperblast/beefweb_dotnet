using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure;

internal static class RequestHandlerExtensions
{
    public static async ValueTask<T> Get<T>(
        this IRequestHandler handler,
        string url,
        QueryParameterCollection? queryParams = null,
        CancellationToken cancellationToken = default)
    {
        return (T)await handler.Get(typeof(T), url, queryParams, cancellationToken);
    }

    public static async IAsyncEnumerable<T> GetEvents<T>(
        this IRequestHandler requestHandler,
        string url,
        QueryParameterCollection? queryParams = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in requestHandler.GetEvents(typeof(T), url, queryParams, cancellationToken))
            yield return (T)item;
    }
}

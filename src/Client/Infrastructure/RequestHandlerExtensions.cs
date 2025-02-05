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
        return (T)await handler.Get(typeof(T), url, queryParams, cancellationToken).ConfigureAwait(false);
    }

    public static ValueTask<object?> Post(
        this IRequestHandler handler,
        string url,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        return handler.Post(null, url, body, cancellationToken);
    }

    public static async ValueTask<T> Post<T>(
        this IRequestHandler handler,
        string url,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        return (T)(await handler.Post(typeof(T), url, body, cancellationToken).ConfigureAwait(false))!;
    }

    public static async IAsyncEnumerable<T> GetEvents<T>(
        this IRequestHandler requestHandler,
        string url,
        QueryParameterCollection? queryParams = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in requestHandler
                           .GetEvents(typeof(T), url, queryParams, cancellationToken)
                           .ConfigureAwait(false))
        {
            yield return (T)item;
        }
    }
}

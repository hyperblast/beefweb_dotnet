using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure;

internal interface IRequestHandler
{
    ValueTask<object> Get(
        Type returnType,
        string url,
        QueryParameterCollection? queryParams = null,
        CancellationToken cancellationToken = default);

    ValueTask<IStreamedResult?> GetStream(
        string url,
        QueryParameterCollection? queryParams = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<object> GetEvents(
        Type itemType,
        string url,
        QueryParameterCollection? queryParams = null,
        CancellationToken cancellationToken = default);

    ValueTask Post(
        string url,
        object? body = null,
        CancellationToken cancellationToken = default);
}

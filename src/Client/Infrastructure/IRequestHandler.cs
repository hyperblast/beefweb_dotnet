using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.Client.Infrastructure;

internal interface IRequestHandler
{
    ValueTask<object?> Get(
        Type returnType,
        string url,
        QueryParameterCollection? queryParams = null,
        JsonSerializerOptions? serializerOptions = null,
        bool allowNullResponse = false,
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

    ValueTask<object?> Post(
        Type? returnType,
        string url,
        object? body = null,
        JsonSerializerOptions? serializerOptions = null,
        bool allowNullResponse = false,
        CancellationToken cancellationToken = default);
}

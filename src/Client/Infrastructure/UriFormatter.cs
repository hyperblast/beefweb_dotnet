using System;

namespace Beefweb.Client.Infrastructure;

internal static class UriFormatter
{
    public static Uri AddTrailingSlash(Uri uri)
    {
        if (uri.AbsolutePath.EndsWith('/'))
            return uri;

        var builder = new UriBuilder(uri);
        builder.Path += "/";
        return builder.Uri;
    }

    public static Uri Format(Uri baseUri, string path, QueryParameterCollection? queryParams = null)
    {
        var uri = new Uri(baseUri, path);
        if (queryParams == null || queryParams.Count == 0)
        {
            return uri;
        }

        var builder = new UriBuilder(uri) { Query = queryParams.ToString() };
        return builder.Uri;
    }
}

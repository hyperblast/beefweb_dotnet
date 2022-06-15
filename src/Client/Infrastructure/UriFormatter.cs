using System;

namespace Beefweb.Client.Infrastructure
{
    public static class UriFormatter
    {
        public static Uri AddTrailingSlash(Uri uri)
        {
            return uri.AbsolutePath.EndsWith('/')
                ? uri
                : new Uri(uri + "/");
        }

        public static Uri Format(Uri baseUri, string path, QueryParameterCollection? queryParams = null)
        {
            if (queryParams == null || queryParams.Count == 0)
                return new Uri(baseUri + path);

            var builder = new UriBuilder(baseUri);
            builder.Path += path;
            builder.Query = queryParams.ToString();
            return builder.Uri;
        }
    }
}
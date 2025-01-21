using System;

namespace Beefweb.CommandLineTool.Services;

public static class UriValidator
{
    public static bool HasHttpScheme(Uri uri)
    {
        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }
}

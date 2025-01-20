using System;

namespace Beefweb.CommandLineTool.Services;

public static class Messages
{
    public static readonly string HttpUrlRequired =
        $"URL should have {Uri.UriSchemeHttp} or {Uri.UriSchemeHttps} scheme.";
}

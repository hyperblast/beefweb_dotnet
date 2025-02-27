using System;

namespace Beefweb.Client;

/// <summary>
/// Credentials for accessing API.
/// </summary>
/// <param name="UserName">User name.</param>
/// <param name="Password">Password.</param>
public record ApiCredentials(string UserName, string Password)
{
    /// <summary>
    /// If true, both <see cref="UserName"/> and <see cref="Password"/> are empty.
    /// </summary>
    public bool IsEmpty => UserName.Length == 0 && Password.Length == 0;

    /// <summary>
    /// Constructs <see cref="ApiCredentials"/> based on <see cref="Uri.UserInfo"/>.
    /// </summary>
    /// <param name="uri">URI to construct credentials from.</param>
    /// <returns>Credentials or null, if <see cref="Uri.UserInfo"/> is empty.</returns>
    public static ApiCredentials? FromUri(Uri uri)
    {
        if (!uri.IsAbsoluteUri || uri.UserInfo.Length == 0)
            return null;

        var parts = uri.UserInfo.Split(':', 2);

        return new ApiCredentials(
            Uri.UnescapeDataString(parts[0]),
            parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty);
    }
}

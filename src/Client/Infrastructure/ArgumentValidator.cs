using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Beefweb.Client.Infrastructure;

internal static partial class ArgumentValidator
{
    [GeneratedRegex("^[a-z0-9_]+$")]
    public static partial Regex IdMatcher();

    public static void ValidatePlaylistId(string? id,
        [CallerArgumentExpression(nameof(id))] string parameterName = "")
    {
        if (id == null || !IdMatcher().IsMatch(id))
            throw new ArgumentException($"Invalid playlist identifier '{id ?? "null"}'.", parameterName);
    }

    public static void ValidateConfigId(string id, [CallerArgumentExpression(nameof(id))] string parameterName = "")
    {
        if (!IdMatcher().IsMatch(id))
            throw new ArgumentException($"Invalid configuration identifier '{id}'.", parameterName);
    }
}

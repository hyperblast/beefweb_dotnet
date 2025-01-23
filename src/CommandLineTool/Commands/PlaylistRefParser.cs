using System;
using System.Globalization;
using Beefweb.Client;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Beefweb.CommandLineTool.Commands;

public class PlaylistRefParser : IValueParser<PlaylistRef>
{
    public Type TargetType => typeof(PlaylistRef);

    object? IValueParser.Parse(string? argName, string? value, CultureInfo culture) => Parse(argName, value, culture);

    public PlaylistRef Parse(string? argName, string? value, CultureInfo culture)
    {
        return value != null ? PlaylistRef.Parse(value) : PlaylistRef.Current;
    }
}

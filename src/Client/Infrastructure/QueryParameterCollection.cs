using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Beefweb.Client.Infrastructure;

internal sealed class QueryParameterCollection : Dictionary<string, object?>
{
    public override string ToString() => Format(this);

    private static string Format(IEnumerable<KeyValuePair<string, object?>> parameters)
    {
        var result = new StringBuilder();

        foreach (var (key, value) in parameters)
        {
            if (value == null)
                continue;

            result.Append(Uri.EscapeDataString(key));
            result.Append('=');
            result.Append(Uri.EscapeDataString(FormatValue(value, key)));
            result.Append('&');
        }

        return result.Length == 0
            ? string.Empty
            : result.ToString(0, result.Length - 1);
    }

    private static string FormatValue(object value, string propertyName)
    {
        return value switch
        {
            string stringValue => stringValue,
            bool boolValue => boolValue ? "true" : "false",
            int intValue => intValue.ToString(CultureInfo.InvariantCulture),
            PlaylistRef playlistRef => playlistRef.ToString(),
            PlaylistItemRange itemRange => itemRange.ToString(),
            IEnumerable<string> stringCollection =>
                string.Join(',', stringCollection.Select(s => EscapeArrayItem(s))),
            _ => throw new NotSupportedException(
                $"Parameter '{propertyName}' has unsupported type '{value.GetType()}'.")
        };
    }

    private static string EscapeArrayItem(string s) => s.Replace(@"\", @"\\").Replace(",", @"\,");
}

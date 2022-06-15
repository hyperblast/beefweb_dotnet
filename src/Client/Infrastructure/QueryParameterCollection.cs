using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Beefweb.Client.Infrastructure
{
    public sealed class QueryParameterCollection : Dictionary<string, object?>
    {
        public override string ToString() => Format(this);

        private static string Format(IEnumerable<KeyValuePair<string, object?>> parameters)
        {
            var result = new StringBuilder();
            foreach (var (key, value) in parameters)
            {
                if (value == null)
                    continue;

                result.Append(WebUtility.UrlEncode(key));
                result.Append('=');
                result.Append(WebUtility.UrlEncode(FormatValue(value, key)));
                result.Append('&');
            }

            if (result.Length == 0)
                return string.Empty;

            result.Length -= 1;
            return result.ToString();
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

        private static string EscapeArrayItem(string s) => s.Replace(@"\", @"\\").Replace(@",", @"\,");
    }
}
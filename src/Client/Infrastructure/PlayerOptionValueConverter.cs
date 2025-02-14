using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beefweb.Client.Infrastructure;

internal sealed class PlayerOptionValueConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.True => reader.GetBoolean(),
            JsonTokenType.False => reader.GetBoolean(),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is int intValue)
        {
            writer.WriteNumberValue(intValue);
        }
        else if (value is bool boolValue)
        {
            writer.WriteBooleanValue(boolValue);
        }
        else
        {
            throw new ArgumentException($"Invalid option value: '{value}'.");
        }
    }
}

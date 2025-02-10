using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beefweb.Client.Infrastructure;

internal sealed class PlaylistRefConverter : JsonConverter<PlaylistRef>
{
    public override PlaylistRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String => reader.GetString()!,
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, PlaylistRef value, JsonSerializerOptions options)
    {
        if (value.Id != null)
            writer.WriteStringValue(value.Id);
        else
            writer.WriteNumberValue(value.Index);
    }
}

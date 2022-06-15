using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beefweb.Client.Infrastructure
{
    public sealed class TimeSpanAsSecondsConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeSpan.FromSeconds(reader.GetDouble());
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.TotalSeconds);
        }
    }
}
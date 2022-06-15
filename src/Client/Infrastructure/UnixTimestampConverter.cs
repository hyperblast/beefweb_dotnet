using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beefweb.Client.Infrastructure
{
    public sealed class UnixTimestampConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Epoch.AddSeconds(reader.GetDouble());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((value - Epoch).TotalSeconds);
        }
    }
}
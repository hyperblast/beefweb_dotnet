using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beefweb.Client.Infrastructure
{
    public sealed class FileSystemEntryTypeConverter : JsonConverter<FileSystemEntryType>
    {
        private const string FileString = "F";
        private const string DirectoryString = "D";

        public override FileSystemEntryType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() switch
            {
                FileString => FileSystemEntryType.File,
                DirectoryString => FileSystemEntryType.Directory,
                _ => throw new NotSupportedException("Invalid FileSystemEntryType value, expected \"F\" or \"D\".")
            };
        }

        public override void Write(Utf8JsonWriter writer, FileSystemEntryType value, JsonSerializerOptions options)
        {
            var stringValue = value switch
            {
                FileSystemEntryType.Directory => DirectoryString,
                FileSystemEntryType.File => FileString,
                _ => throw new NotSupportedException("Invalid FileSystemEntryType value.")
            };

            writer.WriteStringValue(stringValue);
        }
    }
}

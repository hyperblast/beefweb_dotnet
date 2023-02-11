using System.Text.Json;
using Beefweb.Client.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Beefweb.Client
{
    public class SerializationTests
    {
        [Fact]
        public void ShouldDeserializeEnumOption()
        {
            var enumOption = JsonSerializer.Deserialize<PlayerOption>(
                @"{""id"":""test"", ""type"":""enum"", ""value"":1, ""enumNames"":[ ""Off"", ""On"" ]}",
                RequestHandler.SerializerOptions)!;

            enumOption.Should().BeEquivalentTo(new PlayerOption
            {
                Id = "test",
                Type = PlayerOptionType.Enum,
                Value = 1,
                EnumNames = new[] { "Off", "On" }
            });
        }

        [Fact]
        public void ShouldDeserializeBoolOption()
        {
            var enumOption = JsonSerializer.Deserialize<PlayerOption>(
                @"{""id"":""test"", ""type"":""bool"", ""value"":true}",
                RequestHandler.SerializerOptions)!;

            enumOption.Should().BeEquivalentTo(new PlayerOption
            {
                Id = "test",
                Type = PlayerOptionType.Bool,
                Value = true,
                EnumNames = null
            });
        }
    }
}

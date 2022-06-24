using FluentAssertions;
using Xunit;

namespace Beefweb.Client
{
    public class PlaylistRefTests
    {
        [Fact]
        public void ShouldFormat()
        {
            new PlaylistRef(1).ToString().Should().Be("1");
            new PlaylistRef("p2").ToString().Should().Be("p2");
        }

        [Fact]
        public void ShouldParse()
        {
            PlaylistRef.Parse("123").Should().Be(new PlaylistRef(123));
            PlaylistRef.Parse("p321").Should().Be(new PlaylistRef("p321"));
        }
    }
}

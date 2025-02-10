using FluentAssertions;
using Xunit;

namespace Beefweb.Client;

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

    [Fact]
    public void ShouldTryParse()
    {
        PlaylistRef.TryParse("p1", out var pl).Should().BeTrue();
        pl.Should().Be(new PlaylistRef("p1"));

        PlaylistRef.TryParse("current", out pl).Should().BeTrue();
        pl.Should().Be(PlaylistRef.Current);

        PlaylistRef.TryParse("0", out pl).Should().BeTrue();
        pl.Should().Be(new PlaylistRef(0));

        PlaylistRef.TryParse("1", out pl).Should().BeTrue();
        pl.Should().Be(new PlaylistRef(1));

        PlaylistRef.TryParse("", out _).Should().BeFalse();
        PlaylistRef.TryParse("-1", out _).Should().BeFalse();
        PlaylistRef.TryParse("invalid?", out _).Should().BeFalse();
    }
}

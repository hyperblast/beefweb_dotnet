using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class VolumeChangeTests
{
    [Theory]
    [InlineData("1db", VolumeChangeType.Db, 1)]
    [InlineData("-1.1db", VolumeChangeType.Db, -1.1)]
    [InlineData("5%", VolumeChangeType.Percent, 5)]
    [InlineData("-15.5%", VolumeChangeType.Percent, -15.5)]
    [InlineData("38", VolumeChangeType.Linear, 38)]
    [InlineData("-38.7", VolumeChangeType.Linear, -38.7)]
    public void TryParse_Works(string input, VolumeChangeType type, double value)
    {
        VolumeChange.TryParse(input, out var change).Should().BeTrue();
        change.Should().Be(new VolumeChange(type, value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Xdb")]
    [InlineData("-X")]
    [InlineData("-Z%")]
    public void TryParse_Fails(string input)
    {
        VolumeChange.TryParse(input, out _).Should().BeFalse();
    }
}

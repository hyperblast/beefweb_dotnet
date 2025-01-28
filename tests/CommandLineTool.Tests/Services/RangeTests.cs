using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class RangeTests
{
    [Theory]
    [InlineData("", -1, -1)]
    [InlineData("x", -1, -1)]
    [InlineData(":1", -1, -1)]
    [InlineData("1", 1, 1)]
    [InlineData("2:1", 2, 1)]
    [InlineData("3:2", 3, 2)]
    [InlineData("4:", 4, -1)]
    public void TryParse_Works(string input, int start, int count)
    {
        var parsed = Range.TryParse(input, out var range);

        if (start < 0)
        {
            parsed.Should().BeFalse();
            return;
        }

        parsed.Should().BeTrue();
        range.Start.Should().Be(start);
        range.Count.Should().Be(count);
    }
}

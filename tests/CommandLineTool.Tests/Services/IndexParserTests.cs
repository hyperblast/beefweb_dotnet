using System;
using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class IndexParserTests
{
    [Theory]
    [InlineData(false, "1", 0, false)]
    [InlineData(false, "123", 122, false)]
    [InlineData(false, "-1", 0, true)]
    [InlineData(false, "-123", 122, true)]
    [InlineData(true, "0", 0, false)]
    [InlineData(true, "1", 1, false)]
    [InlineData(true, "123", 123, false)]
    [InlineData(true, "-0", 0, true)]
    [InlineData(true, "-1", 1, true)]
    [InlineData(true, "-321", 321, true)]
    public void TryParse_Works(bool zeroBased, string input, int value, bool fromEnd)
    {
        IndexParser.TryParse(input, zeroBased, out var result).Should().BeTrue();
        result.Should().Be(new Index(value, fromEnd));
    }

    [Theory]
    [InlineData(false, "")]
    [InlineData(false, "x")]
    [InlineData(false, "0")]
    [InlineData(false, "-0")]
    [InlineData(true, "")]
    [InlineData(true, "x")]
    public void TryParse_Fails(bool zeroBased, string input)
    {
        IndexParser.TryParse(input, zeroBased, out _).Should().BeFalse();
    }
}

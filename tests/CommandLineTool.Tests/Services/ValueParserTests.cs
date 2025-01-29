using System;
using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class ValueParserTests
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
    public void TryParseIndex_Works(bool zeroBased, string input, int value, bool fromEnd)
    {
        ValueParser.TryParseIndex(input, zeroBased, out var result).Should().BeTrue();
        result.Should().Be(new Index(value, fromEnd));
    }

    [Theory]
    [InlineData(false, "")]
    [InlineData(false, "x")]
    [InlineData(false, "0")]
    [InlineData(false, "-0")]
    [InlineData(true, "")]
    [InlineData(true, "x")]
    public void TryParseIndex_Fails(bool zeroBased, string input)
    {
        ValueParser.TryParseIndex(input, zeroBased, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData(false, "1", 0, false, 0, false)]
    [InlineData(false, "..2", 0, false, 1, false)]
    [InlineData(false, "1..", 0, false, 0, true)]
    [InlineData(false, "1..1", 0, false, 0, false)]
    [InlineData(false, "1..3", 0, false, 2, false)]
    [InlineData(false, "-2..-1", 1, true, 0, true)]
    [InlineData(false, "3..-3", 2, false, 2, true)]
    [InlineData(true, "0", 0, false, 0, false)]
    [InlineData(true, "..2", 0, false, 2, false)]
    [InlineData(true, "0..", 0, false, 0, true)]
    [InlineData(true, "0..0", 0, false, 0, false)]
    [InlineData(true, "0..3", 0, false, 3, false)]
    [InlineData(true, "-0", 0, true, 0, true)]
    [InlineData(true, "-0..", 0, true, 0, true)]
    [InlineData(true, "-0..-0", 0, true, 0, true)]
    [InlineData(true, "-2..-1", 2, true, 1, true)]
    [InlineData(true, "2..-3", 2, false, 3, true)]
    public void TryParseRange_Works(bool zeroBased, string input, int from, bool fromFromEnd, int to, bool toFromEnd)
    {
        ValueParser.TryParseRange(input, zeroBased, out var result).Should().BeTrue();
        result.Should().Be(new Range(new Index(from, fromFromEnd), new Index(to, toFromEnd)));
    }
}

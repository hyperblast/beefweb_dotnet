using System;
using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class PositionParserTests
{
    [Theory]
    [InlineData("0", 0, ' ')]
    [InlineData("1", 1, 's')]
    [InlineData("-100ms", -100, ' ')]
    [InlineData("5s", 5, 's')]
    [InlineData("-10m", -10, 'm')]
    [InlineData("3h", 3, 'h')]
    [InlineData("-9d", -9, 'd')]
    [InlineData("1:1", 61, 's')]
    [InlineData("-03:03", -183, 's')]
    [InlineData("1:2:2", 3722, 's')]
    [InlineData("-1.01:02:03", -90123, 's')]
    public void TryParse_Works(string input, double value, char unit)
    {
        PositionParser.TryParse(input, out var result).Should().BeTrue();
        result.Should().Be(GetTimeSpan(value, unit));
    }

    private static TimeSpan GetTimeSpan(double value, char unit)
    {
        return unit switch
        {
            's' => TimeSpan.FromSeconds(value),
            'm' => TimeSpan.FromMinutes(value),
            'h' => TimeSpan.FromHours(value),
            'd' => TimeSpan.FromDays(value),
            _ => TimeSpan.FromMilliseconds(value),
        };
    }
}

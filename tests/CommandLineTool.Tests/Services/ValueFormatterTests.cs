using System;
using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class ValueFormatterTests
{
    [Fact]
    public void FormatsTrackTime()
    {
        var time1 = new TimeSpan(0, 4, 15);
        time1.FormatAsTrackTime().Should().Be("04:15");

        var time2 = new TimeSpan(2, 3, 7);
        time2.FormatAsTrackTime().Should().Be("02:03:07");

        var time3 = new TimeSpan(1, 2, 3, 7);
        time3.FormatAsTrackTime().Should().Be("1.02:03:07");
    }
}

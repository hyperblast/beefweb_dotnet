using System;
using Beefweb.Client;
using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class ExtensionsTests
{

    [Fact]
    public void ToItemRange()
    {
        var range = new Range(^200, ^100);
        var result = range.GetItemRange(10);
        result.Should().Be(new PlaylistItemRange(0, 0));
    }
}

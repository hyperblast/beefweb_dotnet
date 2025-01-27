using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class ExtensionsTests
{
    [Theory]
    [InlineData("", new int[0])]
    [InlineData(" ", new int[0])]
    [InlineData("\n", new int[0])]
    [InlineData("0", new[] { 0 })]
    [InlineData("1 23", new[] { 1, 23 })]
    [InlineData(" 2\n34", new[] { 2, 34 })]
    [InlineData("3 45 ", new[] { 3, 45 })]
    [InlineData(" 5\r67 ", new[] { 5, 67 })]
    public void ReadIndicesWorks(string input, int[] result)
    {
        var reader = new StringReader(input);
        reader.ReadIndices().Should().BeEquivalentTo(result);
    }

    [Theory]
    [InlineData("x", 0, 0)]
    [InlineData("123x", 0, 3)]
    [InlineData("1\nx", 1, 0)]
    [InlineData("2\r x", 1, 1)]
    public void ReadIndicesFails(string input, int line, int offset)
    {
        var reader = new StringReader(input);
        Action action = () => _ = reader.ReadIndices().ToArray();
        action.Should().Throw<InvalidRequestException>().Which.Message.Should().EndWith($" at {line}:{offset}.");
    }
}

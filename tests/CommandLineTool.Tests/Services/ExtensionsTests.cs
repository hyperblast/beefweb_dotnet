using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    [InlineData("5 66 777 8888 99999", new[] { 5, 66, 777, 8888, 99999 })]
    public async ValueTask ReadIndicesAsync_Works(string input, int[] expectedResult)
    {
        var reader = new StringReader(input);
        var result = await reader.ReadIndicesAsync().ToListAsync();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("x", 0, 0)]
    [InlineData("123x", 0, 3)]
    [InlineData("1\nx", 1, 0)]
    [InlineData("2\r\n x", 1, 1)]
    public async ValueTask ReadIndicesAsync_Fails(string input, int line, int offset)
    {
        var reader = new StringReader(input);
        Func<Task> action = () => reader.ReadIndicesAsync().ToListAsync().AsTask();
        var result = await action.Should().ThrowAsync<InvalidRequestException>();
        result.Which.Message.Should().EndWith($" at {line}:{offset}.");
    }

    [Fact]
    public async ValueTask ReadIndicesAsync_WorksWithLargeInput()
    {
        var input = new StringBuilder();

        for (var i = 0; i < 1024; i++)
        {
            input.Append("123 ");
        }

        var reader = new StringReader(input.ToString());
        var result = await reader.ReadIndicesAsync().ToListAsync();
        result.Should().BeEquivalentTo(Enumerable.Repeat(123, 1024));
    }
}

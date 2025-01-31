using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Beefweb.CommandLineTool.Services;

public class TokenReaderTests
{
    [Theory]
    [InlineData("", new string[0])]
    [InlineData(" ", new string[0])]
    [InlineData("\n", new string[0])]
    [InlineData("0", new[] { "0" })]
    [InlineData("1 23", new[] { "1", "23" })]
    [InlineData(" 2\n34", new[] { "2", "34" })]
    [InlineData("3 45 ", new[] { "3", "45" })]
    [InlineData(" 5\r67 ", new[] {"5", "67" })]
    [InlineData("5 66 777 8888 99999", new[] { "5", "66", "777", "8888", "99999" })]
    public async Task ReadTokensAsync_Works(string input, string[] expectedResult)
    {
        var reader = new StringReader(input);
        var result = await reader.ReadTokensAsync().ToListAsync();
        result.Select(r => r.Value).Should().BeEquivalentTo(expectedResult, o => o.WithStrictOrdering());
    }

    [Fact]
    public async Task ReadTokensAsync_WorksWithLargeInput()
    {
        var input = new StringBuilder();

        for (var i = 0; i < 1024; i++)
        {
            input.Append("123 ");
        }

        var reader = new StringReader(input.ToString());
        var result = await reader.ReadTokensAsync().ToListAsync();
        result.Select(r => r.Value).Should().BeEquivalentTo(Enumerable.Repeat("123", 1024));
    }
}

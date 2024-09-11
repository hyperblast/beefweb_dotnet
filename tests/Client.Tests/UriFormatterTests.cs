using System;
using Beefweb.Client.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Beefweb.Client;

public class UriFormatterTests
{
    const string NoSlash = "https://example.com/foo";
    const string WithSlash = "https://example.com/foo/";

    [Fact]
    public void AddsTrailingSlash()
    {
        UriFormatter.AddTrailingSlash(new Uri(NoSlash)).Should().Be(new Uri(WithSlash));
        UriFormatter.AddTrailingSlash(new Uri(WithSlash)).Should().Be(new Uri(WithSlash));
    }

    [Fact]
    public void FormatsUri()
    {
        var result = UriFormatter.Format(new Uri(WithSlash), "bar", new QueryParameterCollection { ["test"] = "baz" });
        result.Should().Be(new Uri(WithSlash + "bar?test=baz"));
    }
}

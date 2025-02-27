using System;
using FluentAssertions;
using Xunit;

namespace Beefweb.Client;

public class ApiCredentialsTests
{
    [Fact]
    public void FromUri_UnescapesValues()
    {
        const string userName = "User#";
        const string password = "P@ssword";

        var builder = new UriBuilder("http://localhost")
        {
            UserName = userName,
            Password = password
        };

        var uri = builder.Uri;
        var credentials = ApiCredentials.FromUri(uri);
        credentials.Should().NotBeNull();
        credentials!.UserName.Should().Be(userName);
        credentials.Password.Should().Be(password);
    }
}

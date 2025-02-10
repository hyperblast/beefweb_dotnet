using System.Net;
using FluentAssertions;
using Xunit;

namespace Beefweb.Client;

public class PlayerClientExceptionTests
{
    [Fact]
    public void Create_Simple()
    {
        var exception = PlayerClientException.Create(HttpStatusCode.MethodNotAllowed, "Method Not Allowed");
        exception.Message.Should().Be("Response status code does not indicate success: 405 (Method Not Allowed).");
        exception.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        exception.ServerErrorMessage.Should().BeNull();
        exception.ErrorParameterName.Should().BeNull();
    }

    [Fact]
    public void Create_WithServerMessage()
    {
        var exception = PlayerClientException.Create(
            HttpStatusCode.InternalServerError,
            "Internal Server Error",
            "it hits the fan");

        exception.Message.Should().Be(
            "Response status code does not indicate success: 500 (Internal Server Error). Server error: it hits the fan.");

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ServerErrorMessage.Should().Be("it hits the fan");
        exception.ErrorParameterName.Should().BeNull();
    }

    [Fact]
    public void Create_WithServerMessage_WithParameterName()
    {
        var exception = PlayerClientException.Create(
            HttpStatusCode.BadRequest,
            "Bad Request",
            "parameter is required",
            "index");

        exception.Message.Should().Be(
            "Response status code does not indicate success: 400 (Bad Request). Server error: parameter is required. Parameter name: index.");

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ServerErrorMessage.Should().Be("parameter is required");
        exception.ErrorParameterName.Should().Be("index");
    }
}

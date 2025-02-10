using System.Net;
using System.Net.Http;
using Beefweb.Client.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Beefweb.Client;

public class RequestHandlerTests
{
    [Fact]
    public void CreateException_Simple()
    {
        var exception = RequestHandler.CreateException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed));
        exception.Message.Should().Be("Response status code does not indicate success: 405 (Method Not Allowed).");
        exception.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        exception.ServerErrorMessage.Should().BeNull();
        exception.ErrorParameterName.Should().BeNull();
    }

    [Fact]
    public void CreateException_ServerMessage()
    {
        var exception = RequestHandler.CreateException(
            new HttpResponseMessage(HttpStatusCode.InternalServerError),
            "it hits the fan");

        exception.Message.Should().Be(
            "Response status code does not indicate success: 500 (Internal Server Error). Server error: it hits the fan.");

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ServerErrorMessage.Should().Be("it hits the fan");
        exception.ErrorParameterName.Should().BeNull();
    }

    [Fact]
    public void CreateException_ServerMessage_ParameterName()
    {
        var exception = RequestHandler.CreateException(
            new HttpResponseMessage(HttpStatusCode.BadRequest),
            "parameter is required", "index");

        exception.Message.Should().Be(
            "Response status code does not indicate success: 400 (Bad Request). Server error: parameter is required. Parameter name: index.");

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ServerErrorMessage.Should().Be("parameter is required");
        exception.ErrorParameterName.Should().Be("index");
    }
}

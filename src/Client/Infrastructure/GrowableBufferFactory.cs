namespace Beefweb.Client.Infrastructure;

internal sealed class GrowableBufferFactory : IGrowableBufferFactory
{
    public IGrowableBuffer CreateBuffer() => new GrowableBuffer();
}
namespace Beefweb.Client.Infrastructure;

internal interface IGrowableBufferFactory
{
    IGrowableBuffer CreateBuffer();
}
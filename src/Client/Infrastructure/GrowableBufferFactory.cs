namespace Beefweb.Client.Infrastructure
{
    public sealed class GrowableBufferFactory : IGrowableBufferFactory
    {
        public IGrowableBuffer CreateBuffer() => new GrowableBuffer();
    }
}
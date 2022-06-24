using System;

namespace Beefweb.Client.Infrastructure
{
    public interface IGrowableBuffer : IDisposable
    {
        byte[] Data { get; }

        void Grow();
    }
}

using System;

namespace Beefweb.Client.Infrastructure;

internal interface IGrowableBuffer : IDisposable
{
    byte[] Data { get; }

    void Grow();
}
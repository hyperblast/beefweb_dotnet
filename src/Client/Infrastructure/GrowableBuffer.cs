using System;
using System.Buffers;

namespace Beefweb.Client.Infrastructure
{
    internal sealed class GrowableBuffer : IGrowableBuffer
    {
        private const int DefaultInitialSize = 2048;
        private const int DefaultMaxSize = 1024 * 1024;

        private readonly ArrayPool<byte> _pool;
        private readonly int _maxSize;

        public byte[] Data { get; private set; }

        public GrowableBuffer(
            int initialSize = DefaultInitialSize,
            int maxSize = DefaultMaxSize,
            ArrayPool<byte>? pool = null)
        {
            if (initialSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialSize));
            if (maxSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxSize));

            _pool = pool ?? ArrayPool<byte>.Shared;
            _maxSize = maxSize;
            Data = _pool.Rent(initialSize);
        }

        public void Grow()
        {
            var newSize = Data.Length * 2;
            if (newSize > _maxSize)
                throw new InvalidOperationException("Buffer is at maximum size.");

            var newData = _pool.Rent(newSize);

            Buffer.BlockCopy(Data, 0, newData, 0, Data.Length);

            _pool.Return(Data);
            Data = newData;
        }

        public void Dispose()
        {
            var buffer = Data;
            if (buffer == null!)
                return;

            Data = null!;
            _pool.Return(buffer);
        }
    }
}

/*
 * The MIT License (MIT)
Copyright © 2013 Şafak Gür
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */

namespace CorrugatedIron.Comms.Sockets
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Represents a buffer manager that when a buffer is requested, blocks the calling thread
    ///     until a buffer is available.
    /// </summary>
    [DebuggerDisplay("Available: {AvailableBuffers} * {BufferSize}B | Disposed: {IsDisposed}")]
    public sealed class BlockingBufferManager : IDisposable
    {
        /// <summary>
        ///     The full name of the <see cref="BlockingBufferManager" /> type.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly string TypeName = typeof(BlockingBufferManager).FullName;

        /// <summary>
        ///     Size of the buffers provided by the buffer manager.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int _bufferSize;

        /// <summary>
        ///     Data block that provides the underlying storage for the buffers provided by the
        ///     buffer manager.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly byte[] _data;

        /// <summary>
        ///     Zero-based starting indices in <see cref="_data" />, of the available segments.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly BlockingCollection<int> _availableIndices;

        /// <summary>
        ///     Zero-based starting indices in <see cref="_data" />, of the unavailable segments.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ConcurrentDictionary<int, int> _usedIndices;

        /// <summary>
        ///     A value indicating whether the <see cref="BlockingBufferManager.Dispose" /> has
        ///     been called.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isDisposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlockingBufferManager" /> class.
        /// </summary>
        /// <param name="bufferSize">
        ///     Size of the buffers that will be provided by the buffer manager.
        /// </param>
        /// <param name="bufferCount">
        ///     Maximum amount of the buffers that will be concurrently used.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="bufferSize" /> or <paramref name="bufferCount" /> is less than one.
        /// </exception>
        public BlockingBufferManager(int bufferSize, int bufferCount)
        {
            if (bufferSize < 1)
                throw new ArgumentOutOfRangeException(
                    "bufferSize",
                    bufferSize,
                    "Buffer size must not be less than one.");

            if (bufferCount < 1)
                throw new ArgumentOutOfRangeException(
                    "bufferCount",
                    bufferCount,
                    "Buffer count must not be less than one.");

            _bufferSize = bufferSize;
            _data = new byte[bufferSize * bufferCount];
            _availableIndices = new BlockingCollection<int>(bufferCount);
            for (int i = 0; i < bufferCount; i++)
                _availableIndices.Add(bufferSize * i);

            _usedIndices = new ConcurrentDictionary<int, int>(bufferCount, bufferCount);
        }

        /// <summary>
        ///     Gets the size of the buffers provided by the buffer manager.
        /// </summary>
        public int BufferSize
        {
            get { return _bufferSize; }
        }

        /// <summary>
        ///     Gets the number of available buffers provided by the buffer manager.
        /// </summary>
        public int AvailableBuffers
        {
            get
            {
                lock (_availableIndices)
                    return !_isDisposed ? _availableIndices.Count : 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="BlockingBufferManager" /> is
        ///     disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        #region Methods
        /// <summary>
        ///     Gets an available buffer. This method blocks the calling thread until a buffer
        ///     becomes available.
        /// </summary>
        /// <returns>
        ///     An <see cref="ArraySegment&lt;T&gt;" /> with <see cref="BufferSize" /> as its
        ///     count.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="BlockingBufferManager" /> has been disposed.
        /// </exception>
        public ArraySegment<byte> GetBuffer()
        {
            lock (_availableIndices)
                if (_isDisposed)
                    throw new ObjectDisposedException(TypeName);

            int index;
            try
            {
                index = _availableIndices.Take();
            }
            catch (InvalidOperationException)
            {
                throw new ObjectDisposedException(TypeName);
            }

            _usedIndices[index] = index;
            return new ArraySegment<byte>(_data, index, BufferSize);
        }

        /// <summary>
        ///     Releases the specified buffer and makes it available for future use.
        /// </summary>
        /// <param name="buffer">
        ///     Buffer to release.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="buffer" />'s array is null, count is not <see cref="BufferSize" />,
        ///     or the offset is invalid; i.e. not taken from the current buffer manager.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="BlockingBufferManager" /> has been disposed.
        /// </exception>
        public void ReleaseBuffer(ArraySegment<byte> buffer)
        {
            lock (_availableIndices)
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(TypeName);

#if (DEBUG)
                //TODO: Remove this when we sure it works 100%
                Array.Clear(buffer.Array, buffer.Offset, buffer.Count);
#endif
            }

            int offset;
            if (buffer.Array != _data
                || buffer.Count != BufferSize
                || !_usedIndices.TryRemove(buffer.Offset, out offset))
                throw new ArgumentException(
                    "Buffer is not taken from the current buffer manager.",
                    "buffer");
            try
            {
                _availableIndices.Add(offset);
            }
            catch (InvalidOperationException)
            {
                throw new ObjectDisposedException(TypeName);
            }
        }

        /// <summary>
        ///     Releases all resources used by the current instance of
        ///     <see cref="BlockingBufferManager" />. Underlying data block is an exception if it's
        ///     used in unmanaged operations that require pinning the buffer (e.g.
        ///     <see cref="System.Net.Sockets.Socket.ReceiveAsync" />).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2213:DisposableFieldsShouldBeDisposed",
            Justification = "BlockingCollection.Dispose is not thread-safe.",
            MessageId = "availableIndices")]
        public void Dispose()
        {
            lock (_availableIndices)
                if (!_isDisposed)
                {
                    _availableIndices.CompleteAdding();
                    int i;
                    while (_availableIndices.TryTake(out i))
                        _usedIndices[i] = i;

                    _isDisposed = true;
                }
        }
        #endregion
    }
}

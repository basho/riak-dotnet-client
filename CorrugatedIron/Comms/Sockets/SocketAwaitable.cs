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
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    ///     Represents awaitable and re-usable socket arguments.
    /// </summary>
    public sealed class SocketAwaitable : IDisposable
    {
        /// <summary>
        ///     A cached, empty array of bytes.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal static readonly byte[] EmptyArray = new byte[0];

        /// <summary>
        ///     Asynchronous socket arguments for internal use.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SocketAsyncEventArgs _arguments = new SocketAsyncEventArgs();

        /// <summary>
        ///     An object that can be used to synchronize access to the
        ///     <see cref="SocketAwaitable" />.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _syncRoot = new object();

        /// <summary>
        ///     An awaiter that waits the completions of asynchronous socket operations.
        /// </summary>
        private readonly SocketAwaiter _awaiter;

        /// <summary>
        ///     The data buffer segment that holds the transferred bytes.
        /// </summary>
        private ArraySegment<byte> _transferred;

        /// <summary>
        ///     A value indicating whether the <see cref="SocketAwaitable" /> is disposed.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isDisposed;

        /// <summary>
        ///     A value that indicates whether the socket operations using the
        ///     <see cref="SocketAwaitable" /> should capture the current synchronization context
        ///     and attempt to marshall their continuations back to the captured context.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _shouldCaptureContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SocketAwaitable" /> class.
        /// </summary>
        public SocketAwaitable()
        {
            _awaiter = new SocketAwaiter(this);
            _transferred = new ArraySegment<byte>(EmptyArray);
        }

        /// <summary>
        ///     Gets the socket created for accepting a connection with an asynchronous socket
        ///     method.
        /// </summary>
        public Socket AcceptSocket
        {
            get { return this.Arguments.AcceptSocket; }
        }

        /// <summary>
        ///     Gets or sets the data buffer to use with the asynchronous socket methods.
        /// </summary>
        public ArraySegment<byte> Buffer
        {
            get
            {
                lock (_syncRoot)
                    return new ArraySegment<byte>(
                        Arguments.Buffer ?? EmptyArray,
                        Arguments.Offset,
                        Arguments.Count);
            }

            set
            {
                lock (_syncRoot)
                    Arguments.SetBuffer(value.Array ?? EmptyArray, value.Offset, value.Count);
            }
        }

        /// <summary>
        ///     Gets the data buffer segment that holds the transferred bytes.
        /// </summary>
        public ArraySegment<byte> Transferred
        {
            get { return _transferred; }
            internal set { _transferred = value; }
        }

        /// <summary>
        ///     Gets the exception in the case of a connection failure when a
        ///     <see cref="DnsEndPoint" /> was used.
        /// </summary>
        public Exception ConnectByNameError
        {
            get { return Arguments.ConnectByNameError; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether a socket can be reused after a disconnect
        ///     operation.
        /// </summary>
        public bool DisconnectReuseSocket
        {
            get { return Arguments.DisconnectReuseSocket; }
            set { Arguments.DisconnectReuseSocket = value; }
        }

        /// <summary>
        ///     Gets the type of socket operation most recently performed with this context object.
        /// </summary>
        public SocketAsyncOperation LastOperation
        {
            get { return Arguments.LastOperation; }
        }

        /// <summary>
        ///     Gets or sets the remote IP endpoint for an asynchronous operation.
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get { return Arguments.RemoteEndPoint; }
            set { Arguments.RemoteEndPoint = value; }
        }

        /// <summary>
        ///     Gets or sets the behavior of an asynchronous operation.
        /// </summary>
        public SocketFlags SocketFlags
        {
            get { return Arguments.SocketFlags; }
            set { Arguments.SocketFlags = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the socket operations using the
        ///     <see cref="SocketAwaitable" /> should capture the current synchronization context
        ///     and attempt to marshall their continuations back to the captured context.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     A socket operation was already in progress using the current
        ///     <see cref="SocketAwaitable" />.
        /// </exception>
        public bool ShouldCaptureContext
        {
            get
            {
                return _shouldCaptureContext;
            }
            set
            {
                lock (_awaiter.SyncRoot)
                {
                    if (_awaiter.IsCompleted)
                    {
                        _shouldCaptureContext = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("A socket operation is already in progress using the same awaitable arguments.");
                    }
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="SocketAwaitable" /> is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        ///     Gets the asynchronous socket arguments for internal use.
        /// </summary>
        internal SocketAsyncEventArgs Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        ///     Clears the buffer, accepted socket, remote endpoint and socket flags to prepare
        ///     <see cref="SocketAwaitable" /> for pooling.
        /// </summary>
        public void Clear()
        {
            Arguments.AcceptSocket = null;
            Arguments.SocketError = SocketError.Success;
            Arguments.SetBuffer(EmptyArray, 0, 0);
            RemoteEndPoint = null;
            SocketFlags = SocketFlags.None;
            Transferred = new ArraySegment<byte>(EmptyArray);

            // TODO: Remove with SocketAwaitable.UserToken.
            Arguments.UserToken = null;
        }

        /// <summary>
        ///     Gets the awaitable object to await a socket operation.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketAwaiter" /> used to await this <see cref="SocketAwaitable" />.
        /// </returns>
        public SocketAwaiter GetAwaiter()
        {
            return _awaiter;
        }

        /// <summary>
        ///     Releases all resources used by <see cref="SocketAwaitable" />.
        /// </summary>
        public void Dispose()
        {
            lock (_syncRoot)
                if (!IsDisposed)
                {
                    _arguments.Dispose();
                    _isDisposed = true;
                }
        }
    }
}

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
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides an object that waits for the completion of a <see cref="SocketAwaitable" />.
    ///     This class is not thread-safe: It doesn't support multiple concurrent awaiters.
    /// </summary>
    [DebuggerDisplay("Completed: {IsCompleted}")]
    public sealed class SocketAwaiter : INotifyCompletion
    {
        /// <summary>
        ///     A sentinel delegate that does nothing.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Action Sentinel = delegate { };

        /// <summary>
        ///     The asynchronous socket arguments to await.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SocketAwaitable _awaitable;

        /// <summary>
        ///     An object to synchronize access to the awaiter for validations.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _syncRoot = new object();

        /// <summary>
        ///     The continuation delegate that will be called after the current operation is
        ///     awaited.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action _continuation;

        /// <summary>
        ///     A value indicating whether the asynchronous operation is completed.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isCompleted = true;

        /// <summary>
        ///     A synchronization context for marshaling the continuation delegate to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SynchronizationContext _syncContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SocketAwaiter" /> class.
        /// </summary>
        /// <param name="awaitable">
        ///     The asynchronous socket arguments to await.
        /// </param>
        internal SocketAwaiter(SocketAwaitable awaitable)
        {
            _awaitable = awaitable;
            _awaitable.Arguments.Completed += delegate
            {
                var c = _continuation
                    ?? Interlocked.CompareExchange(ref _continuation, Sentinel, null);

                if (c != null)
                {
                    var syncContext = _awaitable.ShouldCaptureContext
                        ? SyncContext
                        : null;

                    Complete();
                    if (syncContext != null)
                        syncContext.Post(s => c.Invoke(), null);
                    else
                        c.Invoke();
                }
            };
        }

        /// <summary>
        ///     Gets a value indicating whether the asynchronous operation is completed.
        /// </summary>
        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        /// <summary>
        ///     Gets an object to synchronize access to the awaiter for validations.
        /// </summary>
        internal object SyncRoot
        {
            get { return _syncRoot; }
        }

        /// <summary>
        ///     Gets or sets a synchronization context for marshaling the continuation delegate to.
        /// </summary>
        internal SynchronizationContext SyncContext
        {
            get { return _syncContext; }
            set { _syncContext = value; }
        }

        /// <summary>
        ///     Gets the result of the asynchronous socket operation.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketError" /> that represents the result of the socket operations.
        /// </returns>
        public SocketError GetResult()
        {
            return _awaitable.Arguments.SocketError;
        }

        /// <summary>
        ///     Gets invoked when the asynchronous operation is completed and runs the specified
        ///     delegate as continuation.
        /// </summary>
        /// <param name="continuation">
        ///     Continuation to run.
        /// </param>
        public void OnCompleted(Action continuation)
        {
            if (_continuation != Sentinel && Interlocked.CompareExchange(
                ref _continuation,
                continuation,
                null) != Sentinel) return;
            Complete();
            if (!_awaitable.ShouldCaptureContext)
            {
                Task.Run(continuation);//.ConfigureAwait(false);
            }
            else
            {
                Task.Factory.StartNew(
                    continuation,
                    CancellationToken.None,
                    TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        ///     Resets this awaiter for re-use.
        /// </summary>
        internal void Reset()
        {
            _awaitable.Arguments.AcceptSocket = null;
            _awaitable.Arguments.SocketError = SocketError.AlreadyInProgress;
            _awaitable.Transferred = new ArraySegment<byte>(SocketAwaitable.EmptyArray);
            _isCompleted = false;
            _continuation = null;
        }

        /// <summary>
        ///     Sets <see cref="IsCompleted" /> to true, nullifies the <see cref="_syncContext" />
        ///     and updates <see cref="SocketAwaitable.Transferred" />.
        /// </summary>
        internal void Complete()
        {
            if (IsCompleted) return;

            var buffer = _awaitable.Buffer;
            _awaitable.Transferred = buffer.Count == 0
                ? buffer
                : new ArraySegment<byte>(
                    buffer.Array,
                    buffer.Offset,
                    _awaitable.Arguments.BytesTransferred);

            if (_awaitable.ShouldCaptureContext)
                _syncContext = null;

            _isCompleted = true;
        }
    }
}

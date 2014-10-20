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
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    ///     Represents a thread-safe pool of awaitable socket arguments.
    /// </summary>
    [DebuggerDisplay("Count: {Count}")]
    public sealed class SocketAwaitablePool
        : ICollection, IDisposable, IEnumerable<SocketAwaitable>
    {
        /// <summary>
        ///     The full name of the <see cref="SocketAwaitablePool" /> type.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly string TypeName = typeof(SocketAwaitablePool).FullName;

        /// <summary>
        ///     A thread-safe, unordered collection of awaitable socket arguments.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ConcurrentBag<SocketAwaitable> _bag;

        /// <summary>
        ///     A value indicating whether the <see cref="SocketAwaitablePool" /> is disposed.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isDisposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SocketAwaitablePool" /> class.
        /// </summary>
        /// <param name="initialCount">
        ///     The initial size of the pool.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="initialCount" /> is less than zero.
        /// </exception>
        public SocketAwaitablePool(int initialCount = 0)
        {
            if (initialCount < 0)
                throw new ArgumentOutOfRangeException(
                    "initialCount",
                    initialCount,
                    "Initial count must not be less than zero.");

            _bag = new ConcurrentBag<SocketAwaitable>();
            for (var i = 0; i < initialCount; i++)
                Add(new SocketAwaitable());
        }

        /// <summary>
        ///     Gets the number of awaitable socket arguments in the
        ///     <see cref="SocketAwaitablePool" />.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_bag)
                    return !IsDisposed ? _bag.Count : 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="SocketAwaitablePool" /> is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                lock (_bag)
                    return _isDisposed;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether access to the <see cref="ICollection" /> is
        ///     synchronized with the <see cref="ICollection.SyncRoot" /> property.
        ///     This property always returns false.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets an object that can be used to synchronize access to the
        ///     <see cref="ICollection" />. This property is not supported.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object ICollection.SyncRoot
        {
            get
            {
                throw new NotSupportedException(
                    "Synchronization using SyncRoot is not supported.");
            }
        }

        /// <summary>
        ///     Adds a <see cref="SocketAwaitable" /> instance to the pool.
        /// </summary>
        /// <param name="awaitable">
        ///     Awaitable socket arguments to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="awaitable" /> is null.
        /// </exception>
        public void Add(SocketAwaitable awaitable)
        {
            if (awaitable == null)
                throw new ArgumentNullException(
                    "awaitable",
                    "Awaitable socket arguments to pull must not be null.");

            lock (_bag)
                if (!IsDisposed)
                    _bag.Add(awaitable);
                else
                    awaitable.Dispose();
        }

        /// <summary>
        ///     Removes and returns a <see cref="SocketAwaitable" /> instance from the pool, if the
        ///     pool has one; otherwise, returns a new <see cref="SocketAwaitable" /> instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketAwaitable" /> instance from the pool, if the pool has one;
        ///     otherwise, a new <see cref="SocketAwaitable" /> instance.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="SocketAwaitablePool" /> has been disposed.
        /// </exception>
        public SocketAwaitable Take()
        {
            SocketAwaitable awaitable;
            lock (_bag)
                if (!IsDisposed)
                    return _bag.TryTake(out awaitable) ? awaitable : new SocketAwaitable();
                else
                    throw new ObjectDisposedException(TypeName);
        }

        /// <summary>
        ///     Copies the pool elements to an existing one-dimensional array, starting at the
        ///     specified offset.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional array of awaitable socket arguments that is the destination of
        ///     the arguments copied from the pool. Array must have zero-based indexing.
        /// </param>
        /// <param name="offset">
        ///     The zero-based index in <paramref name="array" /> of which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="array" /> is not a single-dimensional array of
        ///     <see cref="SocketAwaitable" /> instances.
        ///     -or-
        ///     <paramref name="offset" /> is equal to or greater than the length of
        ///     <paramref name="array" />
        ///     -or-
        ///     The number of elements in the source pool is greater than the available space from
        ///     <paramref name="offset" /> to the end of <paramref name="array" />.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="SocketAwaitablePool" /> has been disposed.
        /// </exception>
        void ICollection.CopyTo(Array array, int offset)
        {
            if (array == null)
                throw new ArgumentNullException("array", "Array must not be null.");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("index", offset, "Index must not be null.");

            if (!(array is SocketAwaitable[]))
            {
                var message = string.Format(
                    "Array must be a single-dimensional array of `{0}`.",
                    typeof(SocketAwaitable).FullName);

                throw new ArgumentException(message, "array");
            }

            lock (_bag)
                if (!IsDisposed)
                    _bag.CopyTo(array as SocketAwaitable[], offset);
                else
                    throw new ObjectDisposedException(TypeName);
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the <see cref="SocketAwaitablePool" />.
        /// </summary>
        /// <returns>
        ///     An enumerator for the contents of the <see cref="SocketAwaitablePool" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="SocketAwaitablePool" /> has been disposed.
        /// </exception>
        public IEnumerator<SocketAwaitable> GetEnumerator()
        {
            if (!IsDisposed)
                return _bag.GetEnumerator();
            throw new ObjectDisposedException(TypeName);
        }

        /// <summary>
        ///     Returns a non-generic enumerator that iterates through the
        ///     <see cref="SocketAwaitablePool" />.
        /// </summary>
        /// <returns>
        ///     An enumerator for the contents of the <see cref="SocketAwaitablePool" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="SocketAwaitablePool" /> has been disposed.
        /// </exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Release all resources used by the <see cref="SocketAwaitablePool" />.
        /// </summary>
        public void Dispose()
        {
            lock (_bag)
                if (!IsDisposed)
                {
                    for (var i = 0; i < Count; i++)
                        Take().Dispose();

                    _isDisposed = true;
                }
        }
    }
}

namespace RiakClient
{
    using System;
    using System.Collections.Generic;
    using Comms;

    /// <summary>
    /// Represents a connection to a Riak node, and allows operations to be performed with that connection.
    /// Partial abstract implementation of <see cref="IRiakEndPoint"/>.
    /// </summary>
    public abstract class RiakEndPoint : IRiakEndPoint
    {
        /// <inheritdoc />
        public TimeSpan RetryWaitTime { get; set; }

        /// <summary>
        /// The max number of retry attempts to make when the client encounters 
        /// <see cref="ResultCode"/>.NoConnections or <see cref="ResultCode"/>.CommunicationError errors.
        /// </summary>
        protected abstract int DefaultRetryCount { get; }

        /// <summary>
        /// Creates a new instance of <see cref="RiakClient"/>.
        /// </summary>
        /// <returns>
        /// A minty fresh client.
        /// </returns>
        public IRiakClient CreateClient()
        {
            return new RiakClient(this) { RetryCount = DefaultRetryCount };
        }

        /// <inheritdoc />
        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult.FromError, retryAttempts);
        }

        /// <inheritdoc />
        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult<TResult>.FromError, retryAttempts);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="RiakEndPoint"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public abstract RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;

        protected abstract void Dispose(bool disposing);

        protected abstract TRiakResult UseConnection<TRiakResult>(Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, bool, TRiakResult> onError, int retryAttempts)
            where TRiakResult : RiakResult;
    }
}

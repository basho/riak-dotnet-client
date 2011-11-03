using System;
using System.Collections.Generic;
using CorrugatedIron.Comms;

namespace CorrugatedIron
{
    public abstract class RiakEndPoint : IRiakEndPoint
    {
        public int RetryWaitTime { get; set; }
        protected abstract int DefaultRetryCount { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CorrugatedIron.RiakClient"/>.
        /// </summary>
        /// <returns>
        /// A minty fresh client.
        /// </returns>
        /// <param name='seed'>
        /// An optional seed to generate the Client Id for the <see cref="CorrugatedIron.RiakClient"/>. Having a unique Client Id is important for
        /// generating good vclocks. For more information about the importance of vector clocks, refer to http://wiki.basho.com/Vector-Clocks.html
        /// </param>
        public IRiakClient CreateClient(string seed = null)
        {
            return new RiakClient(this, seed)
            {
                RetryCount = DefaultRetryCount
            };
        }

        public RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return UseConnection(clientId, useFun, RiakResult.Error, retryAttempts);
        }

        public RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return UseConnection(clientId, useFun, RiakResult<TResult>.Error, retryAttempts);
        }

        protected abstract TRiakResult UseConnection<TRiakResult>(byte[] clientId, Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, TRiakResult> onError, int retryAttempts)
            where TRiakResult : RiakResult;

        public abstract RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;

        public abstract void Dispose();
    }
}
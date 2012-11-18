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
        /// [Obsolete] Creates a new instance of <see cref="CorrugatedIron.RiakClient"/>.
        /// </summary>
        /// <returns>
        /// A minty fresh client.
        /// </returns>
        /// <param name='seed'>
        /// An optional seed to generate the Client Id for the <see cref="CorrugatedIron.RiakClient"/>. Having a unique Client Id is important for
        /// generating good vclocks. For more information about the importance of vector clocks, refer to http://wiki.basho.com/Vector-Clocks.html
        /// </param>
        [Obsolete("Clients no longer need a seed value, use CreateClient() instead")]
        public IRiakClient CreateClient(string seed)
        {
            return new RiakClient(this, seed) { RetryCount = DefaultRetryCount };
        }

        /// <summary>
        /// Creates a new instance of <see cref="CorrugatedIron.RiakClient"/>.
        /// </summary>
        /// <returns>
        /// A minty fresh client.
        /// </returns>
        public IRiakClient CreateClient()
        {
            return new RiakClient(this) { RetryCount = DefaultRetryCount };
        }

        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult.Error, retryAttempts);
        }

        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult<TResult>.Error, retryAttempts);
        }

        protected abstract TRiakResult UseConnection<TRiakResult>(Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, TRiakResult> onError, int retryAttempts)
            where TRiakResult : RiakResult;

        public abstract RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;

        public abstract void Dispose();
    }
}
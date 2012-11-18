using System;
using System.Collections.Generic;
using CorrugatedIron.Comms;

namespace CorrugatedIron
{
    public interface IRiakEndPoint : IDisposable
    {
        int RetryWaitTime { get; set; }

        IRiakClient CreateClient();

        [Obsolete("Clients no longer need a seed value, use CreateClient() instead")]
        IRiakClient CreateClient(string seed);

        RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts);
        RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts);

        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;
    }
}
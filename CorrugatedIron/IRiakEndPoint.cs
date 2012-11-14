using System;
using System.Collections.Generic;
using CorrugatedIron.Comms;

namespace CorrugatedIron
{
    public interface IRiakEndPoint : IDisposable
    {
        int RetryWaitTime { get; set; }
        IRiakClient CreateClient(string seed = null);
        RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts);
        RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts);

        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;
    }
}
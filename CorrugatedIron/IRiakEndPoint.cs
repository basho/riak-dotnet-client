using System;
using System.Collections.Generic;
using CorrugatedIron.Comms;

namespace CorrugatedIron
{
    public interface IRiakEndPoint : IDisposable
    {
        int RetryWaitTime { get; set; }
        IRiakClient CreateClient(string seed = null);
        RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts);
        RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun, int retryAttempts);
        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;
    }
}
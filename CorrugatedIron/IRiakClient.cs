using System;
using System.Collections.Generic;

namespace CorrugatedIron
{
    public interface IRiakClient : IRiakBatchClient
    {
        IRiakAsyncClient Async { get; }

        void Batch(Action<IRiakBatchClient> batchAction);
        T Batch<T>(Func<IRiakBatchClient, T> batchFunction);
        IEnumerable<T> Batch<T>(Func<IRiakBatchClient, IEnumerable<T>> batchFunction);
    }
}
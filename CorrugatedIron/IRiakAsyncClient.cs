using System;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    public interface IRiakAsyncClient : IRiakAsyncBatchClient
    {
        Task Batch(Action<IRiakAsyncBatchClient> batchAction);
        Task<T> Batch<T>(Func<IRiakAsyncBatchClient, T> batchFunction);
        IObservable<T> Batch<T>(Func<IRiakAsyncBatchClient, IObservable<T>> batchFunction);
    }
}
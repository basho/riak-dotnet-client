namespace RiakClient
{
    using System;
    using Commands;

    /// <summary>
    /// Subinterface of <see cref="IRiakBatchClient"/>. 
    /// Adds properties for the Async client and batch operators.
    /// </summary>
    public interface IRiakClient : IRiakBatchClient
    {
        /// <summary>
        /// Fetches an asynchronous version of the client. 
        /// </summary>
        /// <value>The Async version of the client. See <see cref="IRiakAsyncClient"/>.</value>
        IRiakAsyncClient Async { get; }

        /// <summary>
        /// Used to create a batched set of actions to be sent to a Riak cluster.
        /// </summary>
        /// <param name="batchAction">An action that wraps all the operations to batch together.</param>
        void Batch(Action<IRiakBatchClient> batchAction);

        /// <summary>
        /// Used to create a batched set of actions to be sent to a Riak cluster.
        /// </summary>
        /// <typeparam name="T">The <paramref name="batchFun"/>'s return type.</typeparam>
        /// <param name="batchFunction">A func that wraps all the operations to batch together.</param>
        /// <returns>The return value of <paramref name="batchFun"/>.</returns>
        T Batch<T>(Func<IRiakBatchClient, T> batchFunction);

        /// <summary>
        /// Used to execute a command against a Riak cluster.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// A <see cref="RiakResult"/>, which will indicate success. The passed in command will contain the response.
        /// </returns>
        RiakResult Execute(IRiakCommand command);
    }
}

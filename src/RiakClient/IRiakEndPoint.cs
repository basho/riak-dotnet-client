namespace RiakClient
{
    using System;
    using System.Collections.Generic;
    using Comms;

    /// <summary>
    /// Represents a connection to a Riak node, and allows operations to be performed with that connection.
    /// </summary>
    public interface IRiakEndPoint : IDisposable
    {
        /// <summary>
        /// Represents the <see cref="TimeSpan"/> to wait inbetween retry attempts.
        /// </summary>
        TimeSpan RetryWaitTime { get; set; }

        /// <summary>
        /// Creates a new <see cref="IRiakClient"/> instance.
        /// </summary>
        /// <returns>The new <see cref="IRiakClient"/> instance.</returns>
        IRiakClient CreateClient();

        /// <summary>
        /// Executes a delegate function using a <see cref="IRiakConnection"/>, and returns the results. 
        /// Retries if possible for certain error states.
        /// </summary>
        /// <typeparam name="TResult">The type of the result from the <paramref name="useFun"/> parameter.</typeparam>
        /// <param name="useFun">
        /// The delegate function to execute. Takes an <see cref="IRiakConnection"/> as input, and returns a 
        /// <see cref="RiakResult{TResult}"/> as the result of the operation.
        /// </param>
        /// <param name="retryAttempts">The number of times to retry an operation.</param>
        /// <returns>The result of the <paramref name="useFun"/> delegate.</returns>
        RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts);

        /// <summary>
        /// Executes a delegate function using a <see cref="IRiakConnection"/>, and returns the results.
        /// Retries if possible for certain error states.
        /// </summary>
        /// <param name="useFun">
        /// The delegate function to execute. Takes an <see cref="IRiakConnection"/> as input, and returns a 
        /// <see cref="RiakResult"/> as the result of the operation.
        /// </param>
        /// <param name="retryAttempts">The number of times to retry an operation.</param>
        /// <returns>The result of the <paramref name="useFun"/> delegate.</returns>
        RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts);

        /// <summary>
        /// Executes a delegate function using a <see cref="IRiakConnection"/>, and returns the results.
        /// Retries if possible for certain error states.
        /// This method is used over <see cref="UseConnection"/> to keep a connection open to receive streaming results.
        /// </summary>
        /// <typeparam name="TResult">The type of the result from the <paramref name="useFun"/> parameter.</typeparam>
        /// <param name="useFun">
        /// The delegate function to execute. Takes an <see cref="IRiakConnection"/> and an <see cref="Action"/> continuation as input, and returns a 
        /// <see cref="RiakResult{T}"/> containing an <see cref="IEnumerable{TResult}"/> as the results of the operation.
        /// </param>
        /// <param name="retryAttempts">The number of times to retry an operation.</param>
        /// <returns>The results of the <paramref name="useFun"/> delegate.</returns>
        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;
    }
}

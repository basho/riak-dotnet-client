namespace RiakClient.Comms
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the interface for interacting with a Riak Node.
    /// Abstracts explicit connection use away.
    /// </summary>
    public interface IRiakNode : IDisposable
    {
        /// <summary>
        /// Indicates if this node can be marked offline. <b>false</b> if the node points to a load balancer.
        /// </summary>
        /// <returns>
        /// <b>true</b> or <b>false</b> depending on the node's configuration.
        /// </returns>
        bool CanMarkOffline { get; }

        /// <summary>
        /// Execute the <paramref name="useFun"/> delegate using this one of this <see cref="IRiakNode"/>'s connections.
        /// </summary>
        /// <param name="useFun">The delegate function to execute using one of this <see cref="IRiakNode"/>'s connections.</param>
        /// <returns>A <see cref="RiakResult"/> detailing the success or failure of the operation.</returns>
        RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun);

        /// <summary>
        /// Execute the <paramref name="useFun"/> delegate using this one of this <see cref="IRiakNode"/>'s connections.
        /// </summary>
        /// <typeparam name="TResult">The expected return type from the operation.</typeparam>
        /// <param name="useFun">The delegate function to execute using one of this <see cref="IRiakNode"/>'s connections.</param>
        /// <returns>
        /// A <see cref="RiakResult"/> detailing the success or failure of the operation, 
        /// as well as the <typeparamref name="TResult"/> return value.
        /// </returns>
        RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun);

        /// <summary>
        /// Execute the <paramref name="useFun"/> delegate using this one of this <see cref="IRiakNode"/>'s connections,
        /// and keep the connection open.
        /// This method is used over <see cref="UseConnection{TResult}"/> to keep a connection open to receive streaming results.
        /// </summary>
        /// <typeparam name="TResult">The expected return type from the operation.</typeparam>
        /// <param name="useFun">The delegate function to execute using one of this <see cref="IRiakNode"/>'s connections.</param>
        /// <returns>
        /// A <see cref="RiakResult"/> detailing the success or failure of the operation, 
        /// as well as the <typeparamref name="TResult"/> return value.
        /// </returns>
        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun)
            where TResult : RiakResult;
    }
}

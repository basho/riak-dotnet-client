// <copyright file="IRiakNode.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

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

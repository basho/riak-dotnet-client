// <copyright file="IRiakClient.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

namespace RiakClient
{
    using System;

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
    }
}

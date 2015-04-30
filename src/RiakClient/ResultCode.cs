// <copyright file="ResultCode.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient
{
    /// <summary>
    /// Riak operation result codes.
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Cluster was shutting down.
        /// </summary>
        ShuttingDown,

        /// <summary>
        /// The requested riak object was not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Communication error with the cluster.
        /// </summary>
        CommunicationError,

        /// <summary>
        /// An invalid response was received.
        /// </summary>
        InvalidResponse,

        /// <summary>
        /// The cluster is offline.
        /// </summary>
        ClusterOffline,

        /// <summary>
        /// No available connections to make a request.
        /// </summary>
        NoConnections,

        /// <summary>
        /// An exception occurred during a batch operation.
        /// </summary>
        BatchException,

        /// <summary>
        /// The client ran out of retry attempts while trying to process the request.
        /// </summary>
        NoRetries,

        /// <summary>
        /// An HTTP error occurred.
        /// </summary>
        HttpError,

        /// <summary>
        /// An invalid request was performed.
        /// </summary>
        InvalidRequest
    }
}

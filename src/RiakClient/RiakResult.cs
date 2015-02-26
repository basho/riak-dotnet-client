// <copyright file="RiakResult.cs" company="Basho Technologies, Inc.">
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
    /// <summary>
    /// Represents the collection of result information for a Riak operation that has no specific return value.
    /// </summary>
    public class RiakResult
    {
        protected RiakResult()
        {
        }

        /// <summary>
        /// <b>true</b> if the Riak operation was a success, otherwise, <b>false</b>.
        /// </summary>
        public bool IsSuccess { get; protected set; }
        
        /// <summary>
        /// The error message returned from the Riak operation, in the case that the operation was not a success.
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// The <see cref="ResultCode"/> returned from the operation.
        /// </summary>
        public ResultCode ResultCode { get; protected set; }

        internal bool NodeOffline { get; set; }

        internal static RiakResult Success()
        {
            return new RiakResult
            {
                IsSuccess = true,
                ResultCode = ResultCode.Success
            };
        }

        // TODO: add Exception
        internal static RiakResult Error(ResultCode code, string message, bool nodeOffline)
        {
            return new RiakResult
            {
                IsSuccess = false,
                ResultCode = code,
                ErrorMessage = message,
                NodeOffline = nodeOffline
            };
        }
    }
}

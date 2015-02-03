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
    public class RiakResult
    {
        protected RiakResult()
        {
        }

        public bool IsSuccess { get; protected set; }
        
        public string ErrorMessage { get; protected set; }

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

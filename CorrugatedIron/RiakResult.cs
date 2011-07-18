// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

namespace CorrugatedIron
{
    public enum ResultCode
    {
        Success = 0,
        ShuttingDown,
        NotFound,
        CommunicationError,
        InvalidResponse,
        ClusterOffline,
        NoConnections,
        BatchException,
        NoRetries
    }

    public class RiakResult
    {
        public bool IsSuccess { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public ResultCode ResultCode { get; protected set; }

        protected RiakResult()
        {
        }

        public static RiakResult Success()
        {
            return new RiakResult
            {
                IsSuccess = true,
                ResultCode = ResultCode.Success
            };
        }

        public static RiakResult Error(ResultCode code, string message = null)
        {
            return new RiakResult
            {
                IsSuccess = false,
                ResultCode = code,
                ErrorMessage = message
            };
        }
    }

    public class RiakResult<TResult> : RiakResult
    {
        public TResult Value { get; private set; }

        private RiakResult()
        {
        }

        public static RiakResult<TResult> Success(TResult value)
        {
            return new RiakResult<TResult>
            {
                IsSuccess = true,
                ResultCode = ResultCode.Success,
                Value = value
            };
        }

        public static new RiakResult<TResult> Error(ResultCode code, string message = null)
        {
            return new RiakResult<TResult>
            {
                IsSuccess = false,
                ResultCode = code,
                ErrorMessage = message
            };
        }
    }
}

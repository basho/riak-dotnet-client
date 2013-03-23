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
        NoRetries,
        HttpError,
        InvalidRequest
    }

    public class RiakResult
    {
        public bool IsSuccess { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public ResultCode ResultCode { get; protected set; }
        internal bool NodeOffline { get; set; }

        protected RiakResult()
        {
        }

        internal static RiakResult Success()
        {
            return new RiakResult
            {
                IsSuccess = true,
                ResultCode = ResultCode.Success
            };
        }

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

    public class RiakResult<TResult> : RiakResult
    {
        public TResult Value { get; private set; }

        private RiakResult()
        {
        }

        internal static RiakResult<TResult> Success(TResult value)
        {
            return new RiakResult<TResult>
            {
                IsSuccess = true,
                ResultCode = ResultCode.Success,
                Value = value
            };
        }

        internal new static RiakResult<TResult> Error(ResultCode code, string message, bool nodeOffline)
        {
            return new RiakResult<TResult>
            {
                IsSuccess = false,
                ResultCode = code,
                ErrorMessage = message,
                NodeOffline = nodeOffline
            };
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Value != null ? Value.GetHashCode() : 0);
                result = (result * 397) ^ IsSuccess.GetHashCode();
                result = (result * 397) ^ ResultCode.GetHashCode();
                result = (result * 397) ^ NodeOffline.GetHashCode();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }
            if(ReferenceEquals(this, obj))
            {
                return true;
            }
            if(obj.GetType() != typeof(RiakResult<TResult>))
            {
                return false;
            }
            return Equals((RiakResult<TResult>)obj);
        }

        public bool Equals(RiakResult<TResult> other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }
            if(ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Value, Value)
                && Equals(other.IsSuccess, IsSuccess)
                && Equals(other.ResultCode, ResultCode)
                && Equals(other.NodeOffline, NodeOffline);
        }
    }
}
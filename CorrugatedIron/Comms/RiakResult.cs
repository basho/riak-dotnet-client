// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

namespace CorrugatedIron.Comms
{
    public class RiakResult
    {
        public bool IsError { get; protected set; }
        public string ErrorMessage { get; protected set; }

        protected RiakResult()
        {
        }

        public static RiakResult Success()
        {
            return new RiakResult
            {
                IsError = false
            };
        }

        public static RiakResult Error(string message = null)
        {
            return new RiakResult
            {
                IsError = true,
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
                IsError = false,
                Value = value
            };
        }

        public static new RiakResult<TResult> Error(string message = null)
        {
            return new RiakResult<TResult>
            {
                IsError = true,
                ErrorMessage = message
            };
        }
    }
}

// <copyright file="RiakResult.cs" company="Basho Technologies, Inc.">
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
    /// Represents the collection of result information for a Riak operation that has no specific return value.
    /// </summary>
    /// <param name="isSuccess"><b>true</b> if the result represents Success, <b>false</b> otherwise. Defaults to <b>true</b>.</param>
    /// <param name="errorMessage">The error message, if any. Defaults to <b>null</b>.</param>
    /// <param name="resultCode">The <see cref="ResultCode"/>. Defaults to <b>ResultCode.Success</b>.</param>
    public class RiakResult
    {
        private readonly bool isSuccess;
        private readonly string errorMessage;
        private readonly ResultCode resultCode;

        public RiakResult(bool isSuccess = true, string errorMessage = null, ResultCode resultCode = ResultCode.Success)
        {
            this.isSuccess = isSuccess;
            this.errorMessage = errorMessage;
            this.resultCode = resultCode;
        }

        /// <summary>
        /// <b>true</b> if the Riak operation was a success, otherwise, <b>false</b>.
        /// </summary>
        public bool IsSuccess
        {
            get { return isSuccess; }
        }

        /// <summary>
        /// The error message returned from the Riak operation, in the case that the operation was not a success.
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        /// <summary>
        /// The <see cref="ResultCode"/> returned from the operation.
        /// </summary>
        public ResultCode ResultCode
        {
            get { return resultCode; }
        }

        internal bool NodeOffline { get; set; }

        internal static RiakResult Success()
        {
            return new RiakResult();
        }

        // TODO: add Exception
        internal static RiakResult Error(ResultCode code, string message, bool nodeOffline)
        {
            var riakResult = new RiakResult(false, message, code);
            riakResult.NodeOffline = nodeOffline;
            return riakResult;
        }
    }
}

// <copyright file="RiakException.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Exceptions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    // TODO: ensure that subclass is correct
    public class RiakException : Exception
    {
        private readonly string errorMessage;
        private readonly uint errorCode;
        private readonly bool nodeOffline;

        internal RiakException(uint errorCode, string errorMessage, bool nodeOffline)
        {
            this.nodeOffline = nodeOffline;
            this.errorCode = errorCode;
            this.errorMessage = string.Format("Riak returned an error. Code '{0}'. Message: {1}", this.errorCode, errorMessage);
        }

        internal RiakException(string errorMessage, bool nodeOffline)
        {
            this.nodeOffline = nodeOffline;
            this.errorMessage = errorMessage;
        }

        public uint ErrorCode
        {
            get { return this.errorCode; }
        }

        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        public override string Message
        {
            get { return this.errorMessage; }
        }

        public override IDictionary Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "ErrorCode", this.errorCode },
                    { "ErrorMessage", this.errorMessage }
                };
            }
        }

        internal bool NodeOffline
        {
            get { return this.nodeOffline; }
        }
    }
}

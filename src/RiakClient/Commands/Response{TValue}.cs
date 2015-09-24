// <copyright file="Response{TValue}.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace RiakClient.Commands
{
    /// <summary>
    /// Response to a Riak command with a return value.
    /// </summary>
    /// <typeparam name="TValue">The type of the Riak response.</typeparam>
    public class Response<TValue> : Response
    {
        private readonly TValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="value">The response data.</param>
        public Response(TValue value)
            : this(false, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        public Response(bool notFound)
            : base(notFound)
        {
            this.value = default(TValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        /// <param name="value">The response data.</param>
        public Response(bool notFound, TValue value)
            : base(notFound)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="value">The response data.</param>
        public Response(RiakString key, TValue value)
            : base(key)
        {
            this.value = value;
        }

        /// <summary>
        /// The value returned from Riak
        /// </summary>
        /// <value>The value returned from Riak, deserialized.</value>
        public TValue Value
        {
            get { return value; }
        }
    }
}
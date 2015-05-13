// <copyright file="DataTypeResponse{TValue}.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Commands.CRDT
{
    using System;

    /// <summary>
    /// Response to a Riak CRDT command.
    /// </summary>
    /// <typeparam name="TValue">The type of the value stored in this response.</typeparam>
    public class DataTypeResponse<TValue> : Response<TValue>
    {
        private readonly byte[] context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeResponse{TValue}"/> class representing "Not Found".
        /// </summary>
        public DataTypeResponse()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeResponse{TValue}"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="context">The data type context. Necessary to use this if updating a data type with removals.</param>
        /// <param name="value">The value for this response.</param>
        public DataTypeResponse(RiakString key, byte[] context, TValue value)
            : base(key, value)
        {
            this.context = context;
        }

        /// <summary>
        /// If non-null, a context that can be used for subsequent operations that contain removals.
        /// </summary>
        /// <value>A <see cref="Byte"/>[] representing an opaque context.</value>
        public byte[] Context
        {
            get { return context; }
        }
    }
}
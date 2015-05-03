// <copyright file="Response.cs" company="Basho Technologies, Inc.">
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
    /// Response to a CRDT command.
    /// </summary>
    public class Response
    {
        private readonly RiakString key;
        private readonly bool notFound;
        private readonly byte[] context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class representing "Not Found".
        /// </summary>
        public Response()
        {
            this.notFound = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="context">The data type context. Necessary to use this if updating a data type with removals.</param>
        public Response(RiakString key, byte[] context)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "key is required!");
            }
            else
            {
                this.key = key;
            }

            this.notFound = false;
            this.context = context;
        }

        public bool NotFound
        {
            get { return notFound; }
        }

        public RiakString Key
        {
            get { return key; }
        }

        public byte[] Context
        {
            get { return context; }
        }
    }
}
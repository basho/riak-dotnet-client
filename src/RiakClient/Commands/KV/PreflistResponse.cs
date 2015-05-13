// <copyright file="PreflistResponse.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;

    /// <summary>
    /// Response to a <see cref="FetchPreflist"/> or <see cref="UpdatePreflist"/> command.
    /// </summary>
    public class PreflistResponse : Response<IEnumerable<PreflistItem>>
    {
        /// <inheritdoc />
        public PreflistResponse()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreflistResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="context">The data type context. Necessary to use this if updating a data type with removals.</param>
        /// <param name="value">The value of the fetched preflist.</param>
        public PreflistResponse(RiakString key, byte[] context, IEnumerable<PreflistItem> value)
            : base(key, context, value)
        {
        }
    }
}
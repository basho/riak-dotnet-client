// <copyright file="HllResponse.cs" company="Basho Technologies, Inc.">
// Copyright 2016 - Basho Technologies, Inc.
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
    /// <summary>
    /// Response to a <see cref="FetchHll"/> or <see cref="UpdateHll"/> command.
    /// </summary>
    public class HllResponse : DataTypeResponse<long>
    {
        /// <inheritdoc />
        public HllResponse()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HllResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="value">The value of the fetched hyperloglog.</param>
        public HllResponse(RiakString key, long value)
            : base(key, null, value)
        {
        }

        /// <summary>
        /// Get the estimated cardinality of the HyperLogLog.
        /// Alias for Value.
        /// </summary>
        public long Cardinality => Value;
    }
}
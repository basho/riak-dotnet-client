// <copyright file="CounterOperation.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.RiakDt
{
    using Messages;

    /// <summary>
    /// Represents an operation on a Riak Counter data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class CounterOperation : IDtOp
    {
        private readonly long value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterOperation"/> class.
        /// </summary>
        /// <param name="value">The value to initialize the counter to.</param>
        public CounterOperation(long value)
        {
            this.value = value;
        }

        /// <summary>
        /// The value of the counter.
        /// </summary>
        public long Value
        {
            get { return value; }
        }

        /// <inheritdoc/>
        public DtOp ToDtOp()
        {
            return new DtOp
                {
                    counter_op = new CounterOp { increment = value }
                };
        }
    }
}

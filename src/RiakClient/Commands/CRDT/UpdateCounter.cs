// <copyright file="UpdateCounter.cs" company="Basho Technologies, Inc.">
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
    using Messages;

    /// <summary>
    /// Command used to update a Counter in Riak. As a convenience, a builder method
    /// is provided as well as an object with a fluent API for constructing the
    /// update.
    /// See <see cref="UpdateCounter.Builder"/>
    /// <code>
    /// var update = new UpdateCounter.Builder(10)
    ///           .WithBucketType("maps")
    ///           .WithBucket("myBucket")
    ///           .WithKey("map_1")
    ///           .WithReturnBody(true)
    ///           .Build();
    /// </code>
    /// </summary>
    public class UpdateCounter : UpdateCommand<CounterResponse>
    {
        private readonly UpdateCounterOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCounter"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateCounterOptions"/></param>
        /// <inheritdoc />
        public UpdateCounter(UpdateCounterOptions options)
            : base(options)
        {
            this.options = options;
        }

        protected override DtOp GetRequestOp()
        {
            var op = new DtOp();
            op.counter_op = new CounterOp();
            op.counter_op.increment = options.Increment;
            return op;
        }

        protected override CounterResponse CreateResponse(RiakString key, DtUpdateResp resp)
        {
            return new CounterResponse(key, resp.context, resp.counter_value);
        }

        public class Builder :
            UpdateCommandBuilder<UpdateCounter.Builder, UpdateCounter, UpdateCounterOptions, CounterResponse>
        {
            private long increment;

            public Builder()
            {
            }

            public Builder(long increment)
            {
                this.increment = increment;
            }

            public Builder(long increment, Builder source)
                : base(source)
            {
                this.increment = increment;
            }

            public Builder WithIncrement(long increment)
            {
                this.increment = increment;
                return this;
            }

            protected override void PopulateOptions(UpdateCounterOptions options)
            {
                options.Increment = increment;
            }
        }
    }
}
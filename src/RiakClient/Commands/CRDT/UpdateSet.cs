// <copyright file="UpdateSet.cs" company="Basho Technologies, Inc.">
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
    using Messages;
    using Util;

    /// <summary>
    /// Command used to update a Set in Riak. As a convenience, a builder method
    /// is provided as well as an object with a fluent API for constructing the
    /// update.
    /// See <see cref="UpdateSet.Builder"/>
    /// <code>
    /// var update = new UpdateSet.Builder()
    ///           .WithBucketType("maps")
    ///           .WithBucket("myBucket")
    ///           .WithKey("map_1")
    ///           .WithReturnBody(true)
    ///           .Build();
    /// </code>
    /// </summary>
    public class UpdateSet : UpdateCommand<SetResponse>, IRiakCommand
    {
        private readonly UpdateSetOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSet"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateSetOptions"/></param>
        /// <inheritdoc />
        public UpdateSet(UpdateSetOptions options)
            : base(options)
        {
            this.options = options;
        }

        protected override DtOp GetRequestOp()
        {
            var op = new DtOp();
            op.set_op = new SetOp();

            if (EnumerableUtil.NotNullOrEmpty(options.Additions))
            {
                op.set_op.adds.AddRange(options.Additions);
            }

            if (EnumerableUtil.NotNullOrEmpty(options.Removals))
            {
                op.set_op.removes.AddRange(options.Removals);
            }

            return op;
        }

        protected override SetResponse CreateResponse(RiakString key, DtUpdateResp resp)
        {
            return new SetResponse(key, resp.context, resp.set_value);
        }

        public class Builder : UpdateCommandBuilder<UpdateSet, UpdateSetOptions, SetResponse>
        {
            private IEnumerable<byte[]> additions;
            private IEnumerable<byte[]> removals;

            public Builder(IEnumerable<byte[]> additions, IEnumerable<byte[]> removals)
            {
                this.additions = additions;
                this.removals = removals;
            }

            protected override void PopulateOptions(UpdateSetOptions options)
            {
                options.Additions = additions;
                options.Removals = removals;
            }
        }
    }
}
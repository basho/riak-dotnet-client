// <copyright file="PreflistItem.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Commands.KV
{
    using System;

    /// <summary>
    /// Part of the <see cref="PreflistResponse"/> class.
    /// </summary>
    public class PreflistItem
    {
        private readonly RiakString node;
        private readonly long id;
        private readonly bool primary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreflistItem"/> class.
        /// </summary>
        /// <param name="node">A <see cref="RiakString"/> representing the node owning the partition.</param>
        /// <param name="id">The partition ID.</param>
        /// <param name="primary">Will be <b>true</b> if this is a primary node for the partition.</param>
        public PreflistItem(RiakString node, long id, bool primary)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            this.node = node;
            this.id = id;
            this.primary = primary;
        }

        public RiakString Node
        {
            get { return node; }
        }

        public long ID
        {
            get { return id; }
        }

        public bool Primary
        {
            get { return primary; }
        }
    }
}
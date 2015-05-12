// <copyright file="UpdateCommandOptions.cs" company="Basho Technologies, Inc.">
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
    /// <summary>
    /// Represents options for a <see cref="UpdateCommand{TResponse}"/> operation.
    /// </summary>
    public abstract class UpdateCommandOptions : CommandOptions
    {
        private bool returnBody = true;
        private bool includeContext = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandOptions"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Required.</param>
        /// <param name="bucket">The bucket in Riak. Required.</param>
        /// <param name="key">The key in Riak. If <b>null</b>, Riak will generate a key.</param>
        public UpdateCommandOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key, false)
        {
        }

        /// <summary>
        /// The W (write) value to use.
        /// </summary>
        public Quorum W { get; set; }

        /// <summary>
        /// The PW (primary vnode write) value to use.
        /// </summary>
        public Quorum PW { get; set; }

        /// <summary>
        /// The DW (durable write) value to use.
        /// </summary>
        public Quorum DW { get; set; }

        /// <summary>
        /// If true, returns the updated CRDT.
        /// </summary>
        public bool ReturnBody
        {
            get { return returnBody; }
            set { returnBody = value; }
        }

        /// <summary>
        /// The context from a previous fetch. Required for remove operations. 
        /// </summary>
        public byte[] Context { get; set; }

        /// <summary>
        /// Set to <b>false</b> to not return context. Default (and recommended value) is <b>true</b>.
        /// </summary>
        public bool IncludeContext
        {
            get { return includeContext; }
            set { includeContext = value; }
        }

        /// <summary>
        /// Returns to <b>true</b> if this command has removals.
        /// </summary>
        public bool HasRemoves
        {
            get { return GetHasRemoves(); }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ (W != null ? W.GetHashCode() : 0);
                result = (result * 397) ^ (PW != null ? PW.GetHashCode() : 0);
                result = (result * 397) ^ (DW != null ? DW.GetHashCode() : 0);
                result = (result * 397) ^ ReturnBody.GetHashCode();
                result = (result * 397) ^ Context.GetHashCode();
                result = (result * 397) ^ IncludeContext.GetHashCode();
                return result;
            }
        }

        protected abstract bool GetHasRemoves();
    }
}
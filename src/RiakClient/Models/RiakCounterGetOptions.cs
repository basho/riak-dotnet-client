// <copyright file="RiakCounterGetOptions.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
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

namespace RiakClient.Models
{
    using System.Runtime.InteropServices;
    using Messages;

    /// <summary>
    /// A collection of optional settings for fetching a 1.4-style counter from Riak.
    /// </summary>
    [ComVisible(false)]
    public class RiakCounterGetOptions : RiakOptions<RiakCounterGetOptions>
    {
        /// <summary>
        /// Gets or sets basic quorum semantics.
        /// When set to true, Riak will return early in some failure cases.
        /// (eg. when r=1 and you get 2 errors and a success basic_quorum=true would return an error).
        /// Can be used in conjunction when <see cref="NotFoundOk"/>=<b>false</b> to speed up the case a counter 
        /// does not exist, thereby only reading a "quorum" of not-founds instead of "N" not-founds.
        /// </summary>
        /// <value>
        /// Whether basic quorum semantics will be used.
        /// </value>
        /// <remarks>
        /// This property is not typically modified.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#Early-Failure-Return-with-code-basic_quorum-code-
        /// for more information.
        /// </remarks>
        public bool? BasicQuorum { get; set; }

        /// <summary>
        /// Gets or sets notfound_ok semantics.
        /// When set to true, an object not being found on a Riak node will count towards the R count.
        /// </summary>
        /// <value>
        /// The notfound_ok value.
        /// </value>
        /// <remarks>
        /// This property is not typically modified.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#The-Implications-of-code-notfound_ok-code-
        /// for more information.
        /// </remarks>
        public bool? NotFoundOk { get; set; }

        /// <summary>
        /// A fluent setter for the <see cref="BasicQuorum"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#Early-Failure-Return-with-code-basic_quorum-code-
        /// for more information.
        /// </remarks>
        public RiakCounterGetOptions SetBasicQuorum(bool value)
        {
            BasicQuorum = value;
            return this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="NotFoundOk"/> property.
        /// </summary>
        /// <param name="value">The value to set <see cref="NotFoundOk"/> to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#The-Implications-of-code-notfound_ok-code-
        /// for more information.
        /// </remarks>
        public RiakCounterGetOptions SetNotFoundOk(bool value)
        {
            NotFoundOk = value;
            return this;
        }

        internal void Populate(RpbCounterGetReq request)
        {
            if (R != null)
            {
                request.r = R;
            }

            if (Pr != null)
            {
                request.pr = Pr;
            }

            if (BasicQuorum.HasValue)
            {
                request.basic_quorum = BasicQuorum.Value;
            }

            if (NotFoundOk.HasValue)
            {
                request.notfound_ok = NotFoundOk.Value;
            }
        }
    }
}

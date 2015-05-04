// <copyright file="RiakDtFetchOptions.cs" company="Basho Technologies, Inc.">
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
    /// Represents a collection of optional properties when fetching Riak Data Types. 
    /// Each property changes the semantics of the operation slightly. 
    /// </summary>
    public class RiakDtFetchOptions : RiakOptions<RiakDtFetchOptions>
    {
        public RiakDtFetchOptions()
        {
            IncludeContext = true;
            R = Quorum.WellKnown.Default;
            Pr = Quorum.WellKnown.Default;
        }

        /// <summary>
        /// Basic Quorum semantics - whether to return early in some failure cases (eg. when r=1 and you get 2 errors and a success basic_quorum=true would return an error)
        /// </summary>
        /// <value>
        /// Whether basic quorum semantics will be used.
        /// </value>
        public bool? BasicQuorum { get; private set; }

        /// <summary>
        /// Should not found responses from Riak be treated as an OK result for a find operation. 
        /// </summary>
        /// <value>
        /// The notfound_ok value.
        /// </value>
        public bool? NotFoundOk { get; private set; }

        /// <summary>
        /// Whether a sloppy quorum should be used.
        /// </summary>
        /// <remarks><para>Sloppy quorum defers to the first N healthy nodes from the pref list,
        /// which may not always be the first N nodes encountered in the consistent hash ring.
        /// In effect - sloppy quorum allows us to trust hand off nodes in the event of a network
        /// partition or other similarly amusing event.</para>
        /// <para>Specifying PR will override Sloppy Quorum settings.</para>
        /// <para>Refer to http://lists.basho.com/pipermail/riak-users_lists.basho.com/2012-January/007157.html for additional details.</para></remarks>
        public bool? SloppyQuorum { get; private set; }

        /// <summary>
        /// The number of replicas to read when fetching data.
        /// </summary>
        /// <remarks>
        /// Riak will default this property to an <see cref="NVal"/> equivalent of <b>3</b>.
        /// See http://docs.basho.com/riak/latest/theory/concepts/Replication/ for more information.
        /// </remarks>
        public NVal NVal { get; private set; }

        /// <summary>
        /// Determines whether or not the context will be returned. 
        /// For read-only requests or context-free operations, you can set this to false to reduce the size of the response payload.
        /// </summary>
        public bool IncludeContext { get; private set; }

        /// <summary>
        /// Fluent setter for the <see cref="BasicQuorum"/> property.
        /// When set to true, Riak will return early in some failure cases.
        /// (eg. when r=1 and you get 2 errors and a success basic_quorum=true would return an error).
        /// Can be used in conjunction when <see cref="NotFoundOk"/>=<b>false</b> to speed up the case an object 
        /// does not exist, thereby only reading a "quorum" of not-founds instead of "N" not-founds.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakDtFetchOptions SetBasicQuorum(bool value)
        {
            BasicQuorum = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="NotFoundOk"/> property.
        /// Should not found responses from Riak be treated as an OK result for a find operation. 
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakDtFetchOptions SetNotFoundOk(bool value)
        {
            NotFoundOk = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="SloppyQuorum"/> property.
        /// Whether a sloppy quorum should be used.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakDtFetchOptions SetSloppyQuorum(bool value)
        {
            SloppyQuorum = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="NVal"/> property.
        /// The number of replicas to create when storing data.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakDtFetchOptions SetNVal(NVal value)
        {
            NVal = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="IncludeContext"/> property.
        /// Determines whether or not the context will be returned. 
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakDtFetchOptions SetIncludeContext(bool value)
        {
            IncludeContext = value;
            return this;
        }

        internal void Populate(DtFetchReq request)
        {
            request.r = R;
            request.pr = Pr;

            if (BasicQuorum.HasValue)
            {
                request.basic_quorum = BasicQuorum.Value;
            }

            if (NotFoundOk.HasValue)
            {
                request.notfound_ok = NotFoundOk.Value;
            }

            if (Timeout != null)
            {
                request.timeout = (uint)Timeout;
            }

            if (SloppyQuorum.HasValue)
            {
                request.sloppy_quorum = SloppyQuorum.Value;
            }

            if (NVal != null)
            {
                request.n_val = NVal;
            }

            request.include_context = IncludeContext;
        }
    }
}

// <copyright file="RiakDtUpdateOptions.cs" company="Basho Technologies, Inc.">
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
    /// Represents a collection of optional properties when updating Riak Data Types. 
    /// Each property changes the semantics of the operation slightly. 
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class RiakDtUpdateOptions : RiakOptions<RiakDtUpdateOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDtUpdateOptions" /> class.
        /// </summary>
        /// <remarks>
        /// Initializes <see cref="ReturnBody"/> and <see cref="ReturnBody"/> to <b>true</b>.
        /// Initializes the <see cref="RiakOptions{T}.W"/>, <see cref="RiakOptions{T}.Dw"/>, and <see cref="RiakOptions{T}.Pw"/> properties to 
        /// <see cref="Quorum.WellKnown.Default"/>.
        /// </remarks>
        public RiakDtUpdateOptions()
        {
            ReturnBody = true;
            IncludeContext = true;
            W = Quorum.WellKnown.Default;
            Dw = Quorum.WellKnown.Default;
            Pw = Quorum.WellKnown.Default;
        }

        /// <summary>
        /// The option to return the updated body to the client after the operation.
        /// </summary>
        public bool ReturnBody { get; private set; }

        /// <summary>
        /// Whether a sloppy quorum should be used.
        /// </summary>
        /// <remarks><para>Sloppy quorum defers to the first N healthy nodes from the pref list,
        /// which may not always be the first N nodes encountered in the consistent hash ring.
        /// In effect - sloppy quorum allows us to trust hand off nodes in the event of a network
        /// partition or other similarly amusing event.</para>
        /// <para>Specifying PR will override Sloppy Quorum settings.</para>
        /// <para>Refer to http://lists.basho.com/pipermail/riak-users_lists.basho.com/2012-January/007157.html for additional details.</para></remarks>
        public bool SloppyQuorum { get; private set; }

        /// <summary>
        /// The number of replicas to create when storing data.
        /// </summary>
        /// <remarks>
        /// Riak will default this property to an <see cref="NVal"/> equivalent of <b>3</b>.
        /// See http://docs.basho.com/riak/latest/theory/concepts/Replication/ for more information.
        /// </remarks>
        public NVal NVal { get; private set; }

        /// <summary>
        /// The option to return the new context to the client after the operation.
        /// </summary>
        public bool IncludeContext { get; private set; }

        /// <summary>
        /// Fluent setter for the <see cref="ReturnBody"/> property.
        /// Whether the updated object should be returned after the operation.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakDtUpdateOptions SetReturnBody(bool value)
        {
            ReturnBody = value;
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
        public RiakDtUpdateOptions SetSloppyQuorum(bool value)
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
        public RiakDtUpdateOptions SetNVal(NVal value)
        {
            NVal = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="IncludeContext"/> property.
        /// Whether the updated Context should be returned after the operation.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakDtUpdateOptions SetIncludeContext(bool value)
        {
            IncludeContext = value;
            return this;
        }

        internal void Populate(DtUpdateReq request)
        {
            request.w = W;
            request.dw = Dw;
            request.pw = Pw;

            request.return_body = ReturnBody;

            request.timeoutSpecified = false;
            if (Timeout.HasValue)
            {
                request.timeout = (uint)Timeout;
            }

            request.sloppy_quorum = SloppyQuorum;

            if (NVal != null)
            {
                request.n_val = NVal;
            }

            request.include_context = IncludeContext;
        }
    }
}

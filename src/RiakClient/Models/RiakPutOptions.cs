// <copyright file="RiakPutOptions.cs" company="Basho Technologies, Inc.">
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
    using Messages;

    /// <summary>
    /// A collection of optional settings for updating objects from Riak.
    /// </summary>
    public class RiakPutOptions : RiakOptions<RiakPutOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakPutOptions" /> class.
        /// Uses the "default" quorum settings for W, PW, and DW settings.
        /// </summary>
        public RiakPutOptions()
        {
            // TODO - FUTURE: this should probably not default to true
            ReturnBody = true;
            W = Quorum.WellKnown.Default;
            Dw = Quorum.WellKnown.Default;
            Pw = Quorum.WellKnown.Default;
        }

        /// <summary>
        /// After an update, return the body along with the metadata when set to <b>true</b>. 
        /// Default is <b>false</b>.
        /// </summary>
        /// <remarks>
        /// Note that when set to <b>true</b>, Riak will also return all siblings.
        /// </remarks>
        public bool ReturnBody { get; set; }

        /// <summary>
        /// Update the value only if the local vclock matches the server vclock.
        /// </summary>
        /// <remarks>
        /// Note: This does not guarantee any linearizable properties, as a concurrent update could occur
        /// on a different node, which would result in siblings or an overwritten object depending on
        /// your last_write_wins and allow_mult settings.
        /// </remarks>
        public bool IfNotModified { get; set; }

        /// <summary>
        /// Store the value only if there is no object already present with the same <see cref="RiakObjectId"/>.
        /// </summary>
        /// <remarks>
        /// Note: This does not guarantee any linearizable properties, as a concurrent update could occur
        /// on a different node, which would result in siblings or an overwritten object depending on
        /// your last_write_wins and allow_mult settings.
        /// </remarks>
        public bool IfNoneMatch { get; set; }

        /// <summary>
        /// Return the metadata only.
        /// When set to <b>true</b>, Riak will only return the updated object's metadata, and omit its body.
        /// </summary>
        /// <remarks>
        /// This allows you to get the updated metadata back without also returning a potentially large value.
        /// </remarks>
        public bool ReturnHead { get; set; }

        /// <summary>
        /// Fluent setter for the <see cref="IfNotModified"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakPutOptions SetIfNotModified(bool value)
        {
            IfNotModified = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="SetIfNoneMatch"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakPutOptions SetIfNoneMatch(bool value)
        {
            IfNoneMatch = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="SetReturnBody"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakPutOptions SetReturnBody(bool value)
        {
            ReturnBody = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="SetReturnHead"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakPutOptions SetReturnHead(bool value)
        {
            ReturnHead = value;
            return this;
        }

        internal void Populate(RpbPutReq request)
        {
            request.w = W;
            request.pw = Pw;
            request.dw = Dw;
            request.if_not_modified = IfNotModified;
            request.if_none_match = IfNoneMatch;
            request.return_head = ReturnHead;
            request.return_body = ReturnBody;
            request.timeout = (uint)Timeout;
        }
    }
}

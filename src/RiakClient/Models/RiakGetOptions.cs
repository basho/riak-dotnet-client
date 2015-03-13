// <copyright file="RiakGetOptions.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
    /// A collection of optional settings for fetching objects from Riak.
    /// </summary>
    [ComVisible(false)]
    public class RiakGetOptions : RiakOptions<RiakGetOptions>
    {
        private static readonly RiakGetOptions DefaultGetOptions = new RiakGetOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakGetOptions" /> class.
        /// Uses the "default" quorum settings for R and PR settings.
        /// </summary>
        public RiakGetOptions()
        {
            R = Quorum.WellKnown.Default;
            Pr = Quorum.WellKnown.Default;
        }

        /// <summary>
        /// Returns a static readonly instance of the default <see cref="RiakGetOptions"/> option set.
        /// </summary>
        /// <returns>A static readonly instance of the default <see cref="RiakGetOptions"/> option set.</returns>
        public static RiakGetOptions Default
        {
            get { return DefaultGetOptions; }
        }

        /// <summary>
        /// Gets or sets basic quorum semantics.
        /// When set to true, Riak will return early in some failure cases.
        /// (eg. when r=1 and you get 2 errors and a success basic_quorum=true would return an error).
        /// Can be used in conjunction when <see cref="NotFoundOk"/>=<b>false</b> to speed up the case an object 
        /// does not exist, thereby only reading a "quorum" of not-founds instead of "N" not-founds.
        /// </summary>
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
        /// <remarks>
        /// This property is not typically modified.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#The-Implications-of-code-notfound_ok-code-
        /// for more information.
        /// </remarks>
        public bool? NotFoundOk { get; set; }

        /// <summary>
        /// Return only the object metadata, analogous to an HTTP HEAD request.
        /// When set to <b>true</b>, Riak will return the object minus its value.
        /// </summary>
        /// <remarks>
        /// This allows you to get the metadata without a potentially large value.
        /// </remarks>            
        public bool? Head { get; set; }

        /// <summary>
        /// By default single tombstones are not returned by a fetch operations. 
        /// When set to <b>true</b>, this will return a Tombstone if it is present.
        /// </summary>
        /// <remarks>This property is not typically modified.</remarks>
        public bool? DeletedVclock { get; set; }

        /// <summary>
        /// When a vector clock is supplied with this option, only return the object if the vector clocks don't match.
        /// </summary>
        /// <remarks>This property is not typically modified.</remarks>
        public byte[] IfModified { get; set; }

        internal void Populate(RpbGetReq request)
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

            if (Head.HasValue)
            {
                request.head = Head.Value;
            }

            if (DeletedVclock.HasValue)
            {
                request.deletedvclock = DeletedVclock.Value;
            }

            if (IfModified != null)
            {
                request.if_modified = IfModified;
            }

            if (Timeout != null)
            {
                request.timeout = (uint)Timeout;
            }
        }
    }
}

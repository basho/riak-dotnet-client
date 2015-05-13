// <copyright file="RiakDeleteOptions.cs" company="Basho Technologies, Inc.">
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
    /// A collection of optional settings for deleting objects from Riak.
    /// </summary>
    public class RiakDeleteOptions : RiakOptions<RiakDeleteOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDeleteOptions" /> class.
        /// Uses the "default" quorum settings for R, W, PR, PW, DW, and RW settings.
        /// </summary>
        public RiakDeleteOptions()
        {
            R = Quorum.WellKnown.Default;
            W = Quorum.WellKnown.Default;
            Pr = Quorum.WellKnown.Default;
            Pw = Quorum.WellKnown.Default;
            Dw = Quorum.WellKnown.Default;
            Rw = Quorum.WellKnown.Default;
        }

        /// <summary>
        /// The Vclock of the version that is being deleted. 
        /// Use this to prevent deleting objects that have been modified since the last get request.
        /// </summary>
        /// <value>
        /// The vclock.
        /// </value>
        /// <remarks>
        /// Review the information at http://wiki.basho.com/Vector-Clocks.html for additional information on how vector clocks 
        /// are used in Riak.
        /// </remarks>
        public byte[] Vclock { get; set; }

        internal void Populate(RpbDelReq request)
        {
            request.r = R;
            request.pr = Pr;
            request.rw = Rw;
            request.w = W;
            request.pw = Pw;
            request.dw = Dw;
            request.timeout = (uint)Timeout;

            if (Vclock != null)
            {
                request.vclock = Vclock;
            }
        }
    }
}

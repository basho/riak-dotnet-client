// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
// 
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using CorrugatedIron.Messages;
using CorrugatedIron.Util;

namespace CorrugatedIron.Models
{
    public class RiakDeleteOptions
    {
        /// <summary>
        /// The number of replicas that need to agree when retrieving the object.
        /// </summary>
        /// <value>The RW Value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public uint? Rw { get; set; }

        /// <summary>
        /// The Vclock of the version that is being deleted. Use this to prevent deleting objects that have been modified since the last get request.
        /// </summary>
        /// <value>
        /// The vclock.
        /// </value>
        /// <remarks>Review the information at http://wiki.basho.com/Vector-Clocks.html for additional information on how vector clocks 
        /// are used in Riak.</remarks>
        public byte[] Vclock { get; set; }

        /// <summary>
        /// The number of replicas that must return before a delete is considered a succes.
        /// </summary>
        /// <value>
        /// The R value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public uint? R { get; set; }

        /// <summary>
        /// The number of replicas that must respond before a write is considered a success.
        /// </summary>
        /// <value>The W value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public uint? W { get; set; }

        /// <summary>
        /// Primary Read Quorum - the number of replicas that need to be available when retrieving the object.
        /// </summary>
        /// <value>
        /// The primary read quorum. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public uint? Pr { get; set; }

        /// <summary>
        /// Primary Write Quorum - the number of replicas need to be available when the write is attempted.
        /// </summary>
        /// <value>
        /// The primary write quorum. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public uint? Pw { get; set; }

        /// <summary>
        /// Durable writes - the number of replicas that must commit to durable storage before returning a successful response.
        /// </summary>
        /// <value>
        /// The durable write value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public uint? Dw { get; set; }

        public RiakDeleteOptions()
        {
            Rw = RiakConstants.Defaults.RVal;
        }

        internal void Populate(RpbDelReq request)
        {
            if(Rw.HasValue)
            {
                request.rw = Rw.Value;
            }

            if(Vclock != null)
            {
                request.vclock = Vclock;
            }

            if(R.HasValue)
            {
                request.r = R.Value;
            }

            if(W.HasValue)
            {
                request.w = W.Value;
            }

            if(Pr.HasValue)
            {
                request.pr = Pr.Value;
            }

            if(Pw.HasValue)
            {
                request.pw = Pw.Value;
            }

            if(Dw.HasValue)
            {
                request.dw = Dw.Value;
            }
        }
    }
}
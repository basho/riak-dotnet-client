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

using System;
using System.Collections.Generic;
using CorrugatedIron.Containers;
using CorrugatedIron.Extensions;
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
        public Either<uint, string> Rw { get; private set; }

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
        public Either<uint, string> R { get; private set; }

        /// <summary>
        /// The number of replicas that must respond before a write is considered a success.
        /// </summary>
        /// <value>The W value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public Either<uint, string> W { get; private set; }

        /// <summary>
        /// Primary Read Quorum - the number of replicas that need to be available when retrieving the object.
        /// </summary>
        /// <value>
        /// The primary read quorum. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public Either<uint, string> Pr { get; private set; }

        /// <summary>
        /// Primary Write Quorum - the number of replicas need to be available when the write is attempted.
        /// </summary>
        /// <value>
        /// The primary write quorum. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public Either<uint, string> Pw { get; private set; }

        /// <summary>
        /// Durable writes - the number of replicas that must commit to durable storage before returning a successful response.
        /// </summary>
        /// <value>
        /// The durable write value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public Either<uint, string> Dw { get; private set; }

        public RiakDeleteOptions SetRw(uint value)
        {
            return WriteQuorum(value, var => Rw = var);
        }

        public RiakDeleteOptions SetRw(string value)
        {
            return WriteQuorum(value, var => Rw = var);
        }

        public RiakDeleteOptions SetR(uint value)
        {
            return WriteQuorum(value, var => R = var);
        }

        public RiakDeleteOptions SetR(string value)
        {
            return WriteQuorum(value, var => R = var);
        }

        public RiakDeleteOptions SetW(uint value)
        {
            return WriteQuorum(value, var => W = var);
        }

        public RiakDeleteOptions SetW(string value)
        {
            return WriteQuorum(value, var => W = var);
        }

        public RiakDeleteOptions SetPr(uint value)
        {
            return WriteQuorum(value, var => Pr = var);
        }

        public RiakDeleteOptions SetPr(string value)
        {
            return WriteQuorum(value, var => Pr = var);
        }

        public RiakDeleteOptions SetPw(uint value)
        {
            return WriteQuorum(value, var => Pw = var);
        }

        public RiakDeleteOptions SetPw(string value)
        {
            return WriteQuorum(value, var => Pw = var);
        }

        public RiakDeleteOptions SetDw(uint value)
        {
            return WriteQuorum(value, var => Dw = var);
        }

        public RiakDeleteOptions SetDw(string value)
        {
            return WriteQuorum(value, var => Dw = var);
        }

        public RiakDeleteOptions()
        {
            Rw = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            R = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            W = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Pr = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Pw = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Dw = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
        }

        internal void Populate(RpbDelReq request)
        {
            request.r = R.IsLeft ? R.Left : R.Right.ToRpbOption();
            request.pr = Pr.IsLeft ? Pr.Left : Pr.Right.ToRpbOption();
            request.rw = Rw.IsLeft ? Rw.Left : Rw.Right.ToRpbOption();
            request.w = W.IsLeft ? W.Left : W.Right.ToRpbOption();
            request.pw = Pw.IsLeft ? Pw.Left : Pw.Right.ToRpbOption();
            request.dw = Dw.IsLeft ? Dw.Left : Dw.Right.ToRpbOption();

            if(Vclock != null)
            {
                request.vclock = Vclock;
            }
        }

        private RiakDeleteOptions WriteQuorum(string value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(new Either<uint, string>(value));
            return this;
        }

        private RiakDeleteOptions WriteQuorum(uint value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(new Either<uint, string>(value));
            return this;
        }
    }
}
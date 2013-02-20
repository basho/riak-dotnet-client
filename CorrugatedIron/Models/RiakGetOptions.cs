// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using System;
using System.Collections.Generic;
using CorrugatedIron.Containers;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Util;

namespace CorrugatedIron.Models
{
    public class RiakGetOptions
    {
        /// <summary>
        /// The number of replicas that must return before a delete is considered a success.
        /// </summary>
        /// <value>
        /// The R value. Possible values include any integer as well as magic numbers
        /// to indicate 'quorum', 'one', 'all', or 'default'. 
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public Either<uint, string> R { get; private set; }

        /// <summary>
        /// Primary Read Quorum - the number of replicas that need to be available when retrieving the object.
        /// </summary>
        /// <value>
        /// The primary read quorum. Possible values include any integer as well as magic numbers
        /// to indicate 'quorum', 'one', 'all', or 'default'. 
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="CorrugatedIron.Util.RiakConstants.QuorumOptions"/></remarks>
        public Either<uint, string> Pr { get; private set; }

        /// <summary>
        /// Basic Quorum semantics - whether to return early in some failure cases (eg. when r=1 and you get 2 errors and a success basic_quorum=true would return an error)
        /// </summary>
        /// <value>
        /// Whether basic quorum semantics will be used.
        /// </value>
        public bool? BasicQuorum { get; set; }

        /// <summary>
        /// Should not found responses from Riak be treated as an OK result for a find operation. 
        /// </summary>
        /// <value>
        /// The notfound_ok value.
        /// </value>
        public bool? NotFoundOk { get; set; }

        /// <summary>
        /// Should Riak only return object metadata
        /// </summary>
        /// <value>
        /// The head value.
        /// </value>
        /// <remarks>
        /// This allows the user to retrieve the metadata for an otherwise large object value.
        /// </remarks>            
        public bool? Head { get; set; }

        /// <summary>
        /// Should tombstone vclocks be returned?
        /// </summary>
        /// <value>
        /// deletedvclock. A boolean.
        /// </value>
        public bool? DeletedVclock { get; set; }

        /// <summary>
        /// The reference vclock is supplied results will only returned if the vclocks do not match. 
        /// </summary>
        /// <value>
        /// The reference vclock.
        /// </value>
        public byte[] IfModified { get; set; }

        public RiakGetOptions SetR(uint value)
        {
            return WriteQuorum(value, var => R = var);
        }

        public RiakGetOptions SetR(string value)
        {
            return WriteQuorum(value, var => R = var);
        }

        public RiakGetOptions SetPr(uint value)
        {
            return WriteQuorum(value, var => Pr = var);
        }

        public RiakGetOptions SetPr(string value)
        {
            return WriteQuorum(value, var => Pr = var);
        }

        public RiakGetOptions()
        {
            R = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Pr = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
        }

        internal void Populate(RpbGetReq request)
        {
            request.r = R.IsLeft ? R.Left : R.Right.ToRpbOption();
            request.pr = Pr.IsLeft ? Pr.Left : Pr.Right.ToRpbOption();

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
        }

        private RiakGetOptions WriteQuorum(string value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(new Either<uint, string>(value));
            return this;
        }

        private RiakGetOptions WriteQuorum(uint value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(new Either<uint, string>(value));
            return this;
        }
    }
}


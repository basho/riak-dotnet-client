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
using CorrugatedIron.Messages;
using CorrugatedIron.Util;

namespace CorrugatedIron.Models
{
    public class RiakCounterGetOptions
    {
        /// <summary>
        /// The number of primary replicas that must respond before a read is considered a success.
        /// </summary>
        /// <value>The PR value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public uint? PrVal { get; private set; }

        /// <summary>
        /// The number of replicas that must return before a read is considered a succes.
        /// </summary>
        /// <value>
        /// The R value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        public uint? RVal { get; private set; }

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

        public RiakCounterGetOptions SetBasicQuorum(bool value)
        {
            BasicQuorum = value;
            return this;
        }

        public RiakCounterGetOptions SetNotFoundOk(bool value)
        {
            NotFoundOk = value;
            return this;
        }

        public RiakCounterGetOptions SetRVal(uint value)
        {
            return WriteQuorum(value, v => RVal = v);
        }

        public RiakCounterGetOptions SetRVal(string value)
        {
            return WriteQuorum(value, v => RVal = v);
        }

        public RiakCounterGetOptions SetPrVal(uint value)
        {
            return WriteQuorum(value, v => RVal = v);
        }

        public RiakCounterGetOptions SetPrVal(string value)
        {
            return WriteQuorum(value, v => RVal = v);
        }

        private RiakCounterGetOptions WriteQuorum(string value, Action<uint> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(RiakConstants.QuorumOptionsLookup[value]);
            return this;
        }

        private RiakCounterGetOptions WriteQuorum(uint value, Action<uint> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(value);
            return this;
        }

        internal void Populate(RpbCounterGetReq request)
        {
            if (RVal.HasValue)
                request.r = RVal.Value;

            if (PrVal.HasValue)
                request.pr = PrVal.Value;

            if (BasicQuorum.HasValue)
                request.basic_quorum = BasicQuorum.Value;

            if (NotFoundOk.HasValue)
                request.notfound_ok = NotFoundOk.Value;
        }
    }
}

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
    public class RiakCounterUpdateOptions
    {
        /// <summary>
        /// The number of replicas that must respond before a write is considered a success.
        /// </summary>
        public uint? WVal { get; private set; }

        /// <summary>
        /// The number of replicas that must commit to durable storage and respond before a write is considered a success. 
        /// </summary>
        public uint? DwVal { get; private set; }

        /// <summary>
        /// The number of primary replicas that must respond before a write is considered a success.
        /// </summary>
        public uint? PwVal { get; private set; }

        /// <summary>
        /// Whether or not the updated value should be returned from the counter
        /// </summary>
        public bool? ReturnValue { get; private set; }

        public RiakCounterUpdateOptions SetWVal(string value)
        {
            return WriteQuorum(value, v => WVal = v);
        }

        public RiakCounterUpdateOptions SetWVal(uint value)
        {
            return WriteQuorum(value, v => WVal = v);
        }

        public RiakCounterUpdateOptions SetDwVal(string value)
        {
            return WriteQuorum(value, v => DwVal = v);
        }

        public RiakCounterUpdateOptions SetDwVal(uint value)
        {
            return WriteQuorum(value, v => DwVal = v);
        }

        public RiakCounterUpdateOptions SetPwVal(string value)
        {
            return WriteQuorum(value, v => PwVal = v);
        }

        public RiakCounterUpdateOptions SetPwVal(uint value)
        {
            return WriteQuorum(value, v => PwVal = v);
        }

        public RiakCounterUpdateOptions SetReturnValue(bool value)
        {
            ReturnValue = value;
            return this;
        }

        private RiakCounterUpdateOptions WriteQuorum(string value, Action<uint> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(RiakConstants.QuorumOptionsLookup[value]);
            return this;
        }

        private RiakCounterUpdateOptions WriteQuorum(uint value, Action<uint> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(value);
            return this;
        }

        internal void Populate(RpbCounterUpdateReq request)
        {
            if (WVal.HasValue)
                request.w = WVal.Value;

            if (DwVal.HasValue)
                request.dw = DwVal.Value;

            if (PwVal.HasValue)
                request.pw = PwVal.Value;

            if (ReturnValue.HasValue)
                request.returnvalue = ReturnValue.Value;
        }
    }
}

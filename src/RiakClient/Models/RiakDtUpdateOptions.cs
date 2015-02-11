// <copyright file="RiakDtUpdateOptions.cs" company="Basho Technologies, Inc.">
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

    [ComVisible(false)]
    public class RiakDtUpdateOptions : RiakOptions<RiakDtUpdateOptions>
    {
        public RiakDtUpdateOptions()
        {
            ReturnBody = true;
            IncludeContext = true;
            W = Quorum.WellKnown.Default;
            Dw = Quorum.WellKnown.Default;
            Pw = Quorum.WellKnown.Default;
        }

        public bool ReturnBody { get; private set; }

        public bool SloppyQuorum { get; private set; }

        public uint? NVal { get; private set; }

        public bool IncludeContext { get; private set; }

        public RiakDtUpdateOptions SetReturnBody(bool value)
        {
            ReturnBody = value;
            return this;
        }

        public RiakDtUpdateOptions SetSloppyQuorum(bool value)
        {
            SloppyQuorum = value;
            return this;
        }

        public RiakDtUpdateOptions SetNVal(uint value)
        {
            NVal = value;
            return this;
        }

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

            if (Timeout.HasValue)
            {
                request.timeout = (uint)Timeout.Value;
            }

            request.sloppy_quorum = SloppyQuorum;

            if (NVal.HasValue)
            {
                request.n_val = NVal.Value;
            }

            request.include_context = IncludeContext;
        }
    }
}

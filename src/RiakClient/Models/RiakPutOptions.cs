// <copyright file="RiakPutOptions.cs" company="Basho Technologies, Inc.">
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
    public class RiakPutOptions : RiakOptions<RiakPutOptions>
    {
        public RiakPutOptions()
        {
            ReturnBody = true;
            W = Quorum.WellKnown.Default;
            Dw = Quorum.WellKnown.Default;
            Pw = Quorum.WellKnown.Default;
        }

        public bool ReturnBody { get; set; }

        public bool IfNotModified { get; set; }

        public bool IfNoneMatch { get; set; }

        public bool ReturnHead { get; set; }

        public RiakPutOptions SetIfNotModified(bool value)
        {
            IfNotModified = value;
            return this;
        }

        public RiakPutOptions SetIfNoneMatch(bool value)
        {
            IfNoneMatch = value;
            return this;
        }

        public RiakPutOptions SetReturnBody(bool value)
        {
            ReturnBody = value;
            return this;
        }

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

            if (Timeout.HasValue)
            {
                request.timeout = (uint)Timeout.Value;
            }
        }
    }
}
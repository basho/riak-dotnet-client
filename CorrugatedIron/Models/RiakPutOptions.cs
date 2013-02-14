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
    public class RiakPutOptions
    {
        public Either<uint, string> W { get; private set; }
        public Either<uint, string> Dw { get; private set; }
        public Either<uint, string> Pw { get; private set; }
        public bool ReturnBody { get; set; }
        public bool IfNotModified { get; set; }
        public bool IfNoneMatch { get; set; }
        public bool ReturnHead { get; set; }

        public RiakPutOptions SetW(uint value)
        {
            return WriteQuorum(value, var => W = var);
        }

        public RiakPutOptions SetW(string value)
        {
            return WriteQuorum(value, var => W = var);
        }

        public RiakPutOptions SetDw(uint value)
        {
            return WriteQuorum(value, var => Dw = var);
        }

        public RiakPutOptions SetDw(string value)
        {
            return WriteQuorum(value, var => Dw = var);
        }

        public RiakPutOptions SetPw(uint value)
        {
            return WriteQuorum(value, var => Pw = var);
        }

        public RiakPutOptions SetPw(string value)
        {
            return WriteQuorum(value, var => Pw = var);
        }

        public RiakPutOptions()
        {
            ReturnBody = true;
            W = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Dw = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Pw = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
        }

        internal void Populate(RpbPutReq request)
        {
            request.w = W.IsLeft ? W.Left : W.Right.ToRpbOption();
            request.pw = Pw.IsLeft ? Pw.Left : Pw.Right.ToRpbOption();
            request.dw = Dw.IsLeft ? Dw.Left : Dw.Right.ToRpbOption();

            request.if_not_modified = IfNotModified;
            request.if_none_match = IfNoneMatch;
            request.return_head = ReturnHead;
            request.return_body = ReturnBody;
        }

        private RiakPutOptions WriteQuorum(string value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(new Either<uint, string>(value));
            return this;
        }

        private RiakPutOptions WriteQuorum(uint value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(new Either<uint, string>(value));
            return this;
        }
    }
}
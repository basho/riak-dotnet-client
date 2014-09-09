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
    public class RiakDtUpdateOptions
    {
        public Either<uint, string> W { get; private set; }
        public Either<uint, string> Dw { get; private set; }
        public Either<uint, string> Pw { get; private set; }
        public bool ReturnBody { get; private set; }
        public uint? Timeout { get; private set; }
        public bool SloppyQuorum { get; private set; }
        public uint? NVal { get; private set; }
        public bool IncludeContext { get; private set; }

        public RiakDtUpdateOptions SetW(uint value)
        {
            return WriteQuorum(value, var => W = var);
        }

        public RiakDtUpdateOptions SetW(string value)
        {
            return WriteQuorum(value, var => W = var);
        }

        public RiakDtUpdateOptions SetDw(uint value)
        {
            return WriteQuorum(value, var => Dw = var);
        }

        public RiakDtUpdateOptions SetDw(string value)
        {
            return WriteQuorum(value, var => Dw = var);
        }

        public RiakDtUpdateOptions SetPw(uint value)
        {
            return WriteQuorum(value, var => Pw = var);
        }

        public RiakDtUpdateOptions SetPw(string value)
        {
            return WriteQuorum(value, var => Pw = var);
        }

        public RiakDtUpdateOptions SetReturnBody(bool value)
        {
            ReturnBody = value;
            return this;
        }

        public RiakDtUpdateOptions SetTimeout(uint value)
        {
            Timeout = value;
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

        public RiakDtUpdateOptions()
        {
            IncludeContext = true;
            ReturnBody = false;
            W = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Dw = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
            Pw = new Either<uint, string>(RiakConstants.QuorumOptions.Default);
        }

        internal void Populate(DtUpdateReq request)
        {
            request.w = W.IsLeft ? W.Left : W.Right.ToRpbOption();
            request.dw = Dw.IsLeft ? Dw.Left : Dw.Right.ToRpbOption();
            request.pw = Pw.IsLeft ? Pw.Left : Pw.Right.ToRpbOption();

            request.return_body = ReturnBody;

            if (Timeout.HasValue)
                request.timeout = Timeout.Value;

            request.sloppy_quorum = SloppyQuorum;

            if (NVal.HasValue)
                request.n_val = NVal.Value;

            request.include_context = IncludeContext;
        }

        private RiakDtUpdateOptions WriteQuorum(string value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(new Either<uint, string>(value));
            return this;
        }

        private RiakDtUpdateOptions WriteQuorum(uint value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(new Either<uint, string>(value));
            return this;
        }
    }
}

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

using CorrugatedIron.Messages;

namespace CorrugatedIron.Models
{
    public class RiakPutOptions
    {
        public uint? W { get; set; }
        public uint? Dw { get; set; }
        public bool ReturnBody { get; set; }
        public uint? Pw { get; set; }
        public bool IfNotModified { get; set; }
        public bool IfNoneMatch { get; set; }
        public bool ReturnHead { get; set; }

        public RiakPutOptions()
        {
            ReturnBody = true;
        }

        internal void Populate(RpbPutReq request)
        {
            if(W.HasValue)
            {
                request.w = W.Value;
            }
            if(Dw.HasValue)
            {
                request.dw = Dw.Value;
            }
            if(Pw.HasValue)
            {
                request.pw = Pw.Value;
            }

            request.if_not_modified = IfNotModified;
            request.if_none_match = IfNoneMatch;
            request.return_head = ReturnHead;
            request.return_body = ReturnBody;
        }
    }
}
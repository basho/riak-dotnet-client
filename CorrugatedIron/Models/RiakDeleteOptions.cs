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

namespace CorrugatedIron
{
    public class RiakDeleteOptions
    {
        public uint? Rw { get; set; }
        public byte[] Vclock { get; set; }
        public uint? R { get; set; }
        public uint? W { get; set; }
        public uint? Pr { get; set; }
        public uint? Pw { get; set; }
        public uint? Dw { get; set; }                public RiakDeleteOptions() {            Rw = RiakConstants.Defaults.RVal;        }
        
        internal void Populate(RpbDelReq request)
        {
            if (Rw.HasValue)
            {
                request.Rw = Rw.Value;
            }
            
            if (Vclock != null)
            {
                request.Vclock = Vclock;
            }
            
            if (R.HasValue)
            {
                request.R = R.Value;
            }
            
            if (W.HasValue)
            {
                request.W = W.Value;
            }
            
            if (Pr.HasValue)
            {
                request.Pr = Pr.Value;
            }
            
            if (Pw.HasValue)
            {
                request.Pw = Pw.Value;
            }
            
            if (Dw.HasValue)
            {
                request.Dw = Dw.Value;
            }
        }
    }
}


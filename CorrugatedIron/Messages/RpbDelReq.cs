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
using System.ComponentModel;
using ProtoBuf;

namespace CorrugatedIron.Messages
{
    [Serializable]
    [ProtoContract(Name = "RpbDelReq")]
    internal class RpbDelReq
    {
        [ProtoMember(1, IsRequired = true, Name = "bucket", DataFormat = DataFormat.Default)]
        internal byte[] Bucket { get; set; }

        [ProtoMember(2, IsRequired = true, Name = "key", DataFormat = DataFormat.Default)]
        internal byte[] Key { get; set; }

        [ProtoMember(3, IsRequired = false, Name = "rw", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint Rw { get; set; }

        
        /*     optional bytes vclock = 4;
                optional uint32 r = 5;
                optional uint32 w = 6;
                optional uint32 pr = 7;
                optional uint32 pw = 8;
                optional uint32 dw = 9;
        */
        
        [ProtoMember(4, IsRequired = false, Name = "vclock", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] Vclock { get; set; }
        
        [ProtoMember(5, IsRequired = false, Name = "r", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint R { get; set; }
        
        [ProtoMember(6, IsRequired = false, Name = "w", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint W { get; set; }
        
        [ProtoMember(7, IsRequired = false, Name = "pr", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint Pr { get; set; }
        
        [ProtoMember(8, IsRequired = false, Name = "pw", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint Pw { get; set; }
        
        [ProtoMember(9, IsRequired = false, Name = "dw", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint Dw { get; set; }
    }
}

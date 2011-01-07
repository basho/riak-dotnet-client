// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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
    [ProtoContract(Name = "RpbGetReq")]
    public class RpbGetReq
    {
        [ProtoMember(1, IsRequired = true, Name = "bucket", DataFormat = DataFormat.Default)]
        public byte[] Bucket { get; set; }

        [ProtoMember(2, IsRequired = true, Name = "key", DataFormat = DataFormat.Default)]
        public byte[] Key { get; set; }

        [ProtoMember(3, IsRequired = false, Name = "r", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        public uint R { get; set; }
    }
}

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
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace CorrugatedIron.Messages
{
    [Serializable]
    [ProtoContract(Name = "RpbContent")]
    public class RpbContent
    {
        public RpbContent()
        {
            Links = new List<RpbLink>();
            UserMeta = new List<RpbPair>();
        }

        [ProtoMember(1, IsRequired = true, Name = "value", DataFormat = DataFormat.Default)]
        public byte[] Value { get; set; }

        [ProtoMember(2, IsRequired = false, Name = "content_type", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        public byte[] ContentType { get; set; }

        [ProtoMember(3, IsRequired = false, Name = "charset", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        public byte[] CharacterSet { get; set; }

        [ProtoMember(4, IsRequired = false, Name = "content_encoding", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        public byte[] ContentEncoding { get; set; }

        [ProtoMember(5, IsRequired = false, Name = "vtag", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        public byte[] VTag { get; set; }

        [ProtoMember(6, Name = "links", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        public List<RpbLink> Links { get; private set; }

        [ProtoMember(7, IsRequired = false, Name = "last_mod", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        public uint LastMod { get; set; }

        [ProtoMember(8, IsRequired = false, Name = "last_mod_usecs", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        public uint LastModUSecs { get; set; }

        [ProtoMember(9, Name = "usermeta", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        public List<RpbPair> UserMeta { get; private set; }
    }
}

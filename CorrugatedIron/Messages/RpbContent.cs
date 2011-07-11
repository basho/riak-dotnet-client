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
using System.ComponentModel;
using ProtoBuf;

namespace CorrugatedIron.Messages
{
    [Serializable]
    [ProtoContract(Name = "RpbContent")]
    internal class RpbContent
    {
        internal RpbContent()
        {
            Links = new List<RpbLink>();
            UserMeta = new List<RpbPair>();
        }

        [ProtoMember(1, IsRequired = true, Name = "value", DataFormat = DataFormat.Default)]
        internal byte[] Value { get; set; }

        [ProtoMember(2, IsRequired = false, Name = "content_type", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] ContentType { get; set; }

        [ProtoMember(3, IsRequired = false, Name = "charset", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] CharacterSet { get; set; }

        [ProtoMember(4, IsRequired = false, Name = "content_encoding", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] ContentEncoding { get; set; }

        [ProtoMember(5, IsRequired = false, Name = "vtag", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] VTag { get; set; }

        [ProtoMember(6, Name = "links", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal List<RpbLink> Links { get; set; }

        [ProtoMember(7, IsRequired = false, Name = "last_mod", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint LastMod { get; set; }

        [ProtoMember(8, IsRequired = false, Name = "last_mod_usecs", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint LastModUSecs { get; set; }

        [ProtoMember(9, Name = "usermeta", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal List<RpbPair> UserMeta { get; set; }
    }
}

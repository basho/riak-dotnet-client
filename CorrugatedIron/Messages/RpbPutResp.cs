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
    [ProtoContract(Name = "RpbPutResp")]
    internal class RpbPutResp
    {
        public RpbPutResp()
        {
            Content = new List<RpbContent>();
        }

        [ProtoMember(1, Name = "contents", DataFormat = DataFormat.Default)]
        internal List<RpbContent> Content { get; private set; }

        [ProtoMember(2, IsRequired = false, Name = "vclock", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] VectorClock { get; set; }
        
        [ProtoMember(3, IsRequired = false, Name = "key", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] Key { get; set; }
    }
}

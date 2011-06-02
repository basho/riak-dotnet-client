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
using CorrugatedIron.Extensions;
using ProtoBuf;

namespace CorrugatedIron.Messages
{
    [Serializable]
    [ProtoContract(Name = "RpbListKeysResp")]
    public class RpbListKeysResp
    {
        public RpbListKeysResp()
        {
            Keys = new List<byte[]>();
        }

        [ProtoMember(1, Name = "keys", DataFormat = DataFormat.Default)]
        public List<byte[]> Keys { get; set; }

        [ProtoMember(2, IsRequired = false, Name = "done", DataFormat = DataFormat.Default)]
        [DefaultValue(default(bool))]
        public bool Done { get; set; }
        
        public List<string> KeyNames {
            get {
                var keys = new List<string>();
                Keys.ForEach(k => keys.Add(k.FromRiakString()));
                
                return keys;
            }
        }
    }
}

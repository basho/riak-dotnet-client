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
    [ProtoContract(Name = "RpbMapRedResp")]
    internal class RpbMapRedResp
    {
        [ProtoMember(1, IsRequired = true, Name = "phase", DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(default(uint))]
        internal uint Phase { get; set; }

        [ProtoMember(2, IsRequired = false, Name = "response", DataFormat = DataFormat.Default)]
        [DefaultValue(null)]
        internal byte[] Response { get; set; }

        [ProtoMember(3, IsRequired = false, Name = "done", DataFormat = DataFormat.Default)]
        [DefaultValue(default(bool))]
        internal bool Done { get; set; }
        
        public override int GetHashCode ()
        {
            unchecked
            {
                int result = Phase.GetHashCode();
                result = (result*397) ^ (Response != null ? Response.GetHashCode() : 0);
                result = (result*397) ^ Done.GetHashCode();
                return result;
            }
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(RpbMapRedResp)) return false;
            return Equals((RpbMapRedResp)obj);
        }

        public bool Equals(RpbMapRedResp other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Phase, Phase) && Equals(other.Response, Response) && Equals(other.Done, Done);
        }
    }
}

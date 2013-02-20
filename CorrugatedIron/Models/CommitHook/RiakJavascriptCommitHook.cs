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

using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.CommitHook
{
    public class RiakJavascriptCommitHook : RiakCommitHook, IRiakPreCommitHook
    {
        public string Name { get; private set; }

        public RiakJavascriptCommitHook(string name)
        {
            Name = name;
        }

        protected bool Equals(RiakJavascriptCommitHook other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RiakJavascriptCommitHook)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(RiakJavascriptCommitHook left, RiakJavascriptCommitHook right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RiakJavascriptCommitHook left, RiakJavascriptCommitHook right)
        {
            return !Equals(left, right);
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("name", Name);
            writer.WriteEndObject();
        }
    }
}

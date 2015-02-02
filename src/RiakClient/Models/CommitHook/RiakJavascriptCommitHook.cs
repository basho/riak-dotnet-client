// <copyright file="RiakJavascriptCommitHook.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
// </copyright>

namespace RiakClient.Models.CommitHook
{
    using Extensions;
    using Messages;
    using Newtonsoft.Json;

    public class RiakJavascriptCommitHook : RiakCommitHook, IRiakPreCommitHook
    {
        private readonly string name;

        public RiakJavascriptCommitHook(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public static bool operator ==(RiakJavascriptCommitHook left, RiakJavascriptCommitHook right)
        {
            return RiakJavascriptCommitHook.Equals(left, right);
        }

        public static bool operator !=(RiakJavascriptCommitHook left, RiakJavascriptCommitHook right)
        {
            return !RiakJavascriptCommitHook.Equals(left, right);
        }

        public override bool Equals(RiakCommitHook other)
        {
            return Equals(other as RiakJavascriptCommitHook);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as RiakJavascriptCommitHook);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("name", name);
            writer.WriteEndObject();
        }

        public override RpbCommitHook ToRpbCommitHook()
        {
            return new RpbCommitHook { name = name.ToRiakString() };
        }

        private bool Equals(RiakJavascriptCommitHook other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            return string.Equals(name, other.Name);
        }
    }
}

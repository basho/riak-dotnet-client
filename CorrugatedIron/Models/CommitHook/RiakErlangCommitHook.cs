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
    public class RiakErlangCommitHook : RiakCommitHook, IRiakPreCommitHook, IRiakPostCommitHook
    {
        internal static RiakErlangCommitHook RiakSearchCommitHook;

        public string Module { get; private set; }
        public string Function { get; private set; }

        static RiakErlangCommitHook()
        {
            RiakSearchCommitHook = new RiakErlangCommitHook("riak_search_kv_hook", "precommit");
        }

        public RiakErlangCommitHook(string module, string function)
        {
            Module = module;
            Function = function;
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("mod", Module);
            writer.WriteProperty("fun", Function);
            writer.WriteEndObject();
        }

        protected bool Equals(RiakErlangCommitHook other)
        {
            return string.Equals(Module, other.Module) && string.Equals(Function, other.Function);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((RiakErlangCommitHook)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Module.GetHashCode() * 397) ^ Function.GetHashCode();
            }
        }

        public static bool operator ==(RiakErlangCommitHook left, RiakErlangCommitHook right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RiakErlangCommitHook left, RiakErlangCommitHook right)
        {
            return !Equals(left, right);
        }
    }
}

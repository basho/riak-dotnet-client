// <copyright file="RiakErlangCommitHook.cs" company="Basho Technologies, Inc.">
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

    public class RiakErlangCommitHook : RiakCommitHook, IRiakPreCommitHook, IRiakPostCommitHook
    {
        internal static readonly RiakErlangCommitHook RiakLegacySearchCommitHook;
        private readonly string module;
        private readonly string function;

        static RiakErlangCommitHook()
        {
            RiakLegacySearchCommitHook = new RiakErlangCommitHook("riak_search_kv_hook", "precommit");
        }

        public RiakErlangCommitHook(string module, string function)
        {
            this.module = module;
            this.function = function;
        }

        public string Module
        {
            get { return module; }
        }

        public string Function
        {
            get { return function; }
        }

        public static bool operator ==(RiakErlangCommitHook left, RiakErlangCommitHook right)
        {
            return RiakErlangCommitHook.Equals(left, right);
        }

        public static bool operator !=(RiakErlangCommitHook left, RiakErlangCommitHook right)
        {
            return !RiakErlangCommitHook.Equals(left, right);
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("mod", module);
            writer.WriteProperty("fun", function);
            writer.WriteEndObject();
        }

        public override RpbCommitHook ToRpbCommitHook()
        {
            return new RpbCommitHook
                {
                    modfun = new RpbModFun
                        {
                            function = function.ToRiakString(),
                            module = module.ToRiakString()
                        }
                };
        }

        public override bool Equals(RiakCommitHook other)
        {
            return Equals(other as RiakErlangCommitHook);
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

            return Equals(obj as RiakErlangCommitHook);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (module.GetHashCode() * 397) ^ this.function.GetHashCode();
            }
        }

        private bool Equals(RiakErlangCommitHook other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            return string.Equals(module, other.Module) &&
                string.Equals(function, other.Function);
        }
    }
}

// <copyright file="RiakErlangCommitHook.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

    /// <summary>
    /// Represents an Erlang-based commit hook.
    /// </summary>
    public class RiakErlangCommitHook : RiakCommitHook, IRiakPreCommitHook, IRiakPostCommitHook
    {
        internal static readonly RiakErlangCommitHook RiakLegacySearchCommitHook;
        private readonly string module;
        private readonly string function;

        static RiakErlangCommitHook()
        {
            RiakLegacySearchCommitHook = new RiakErlangCommitHook("riak_search_kv_hook", "precommit");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakErlangCommitHook" /> class.
        /// </summary>
        /// <param name="module">The Erlang module that contains the hook function.</param>
        /// <param name="function">The Erlang function to execute for the hook.</param>
        public RiakErlangCommitHook(string module, string function)
        {
            this.module = module;
            this.function = function;
        }

        /// <summary>
        /// The Erlang module that contains the hook function.
        /// </summary>
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// The Erlang function to execute for the hook.
        /// </summary>
        public string Function
        {
            get { return function; }
        }

        /// <summary>
        /// Determines whether the one object is equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakIndexId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakIndexId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator ==(RiakErlangCommitHook left, RiakErlangCommitHook right)
        {
            return RiakErlangCommitHook.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the one object is <b>not</b> equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakObjectId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakObjectId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is <b>not</b> equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator !=(RiakErlangCommitHook left, RiakErlangCommitHook right)
        {
            return !RiakErlangCommitHook.Equals(left, right);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("mod", module);
            writer.WriteProperty("fun", function);
            writer.WriteEndObject();
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(RiakCommitHook other)
        {
            return Equals(other as RiakErlangCommitHook);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
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

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (module.GetHashCode() * 397) ^ this.function.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
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

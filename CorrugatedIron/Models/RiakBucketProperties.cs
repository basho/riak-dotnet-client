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

using CorrugatedIron.Containers;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models.CommitHook;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CorrugatedIron.Models
{
    public class RiakBucketProperties
    {
        // At the moment, only the NVal and AllowMult can be set via the PBC
        // so if the request has any other value set, we can't use that interface.
        // We check those values and if they're missing we go with PBC as it's
        // substantially quicker.
        public bool CanUsePbc
        {
            get
            {
                return !LastWriteWins.HasValue
                && RVal == null
                && RwVal == null
                && DwVal == null
                && WVal == null
                && PrVal == null
                && PwVal == null
                && PreCommitHooks == null
                && PostCommitHooks == null
                && string.IsNullOrEmpty(Backend);
            }
        }

        public bool? LastWriteWins { get; private set; }
        public uint? NVal { get; private set; }
        public bool? AllowMultiple { get; private set; }
        public string Backend { get; private set; }
        public List<IRiakPreCommitHook> PreCommitHooks { get; private set; }
        public List<IRiakPostCommitHook> PostCommitHooks { get; private set; }
        public bool? NotFoundOk { get; private set; }
        public bool? BasicQuorum { get; private set; }

        /// <summary>
        /// The number of replicas that must return before a read is considered a succes.
        /// </summary>
        /// <value>
        /// The R value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        public Either<uint, string> RVal { get; private set; }

        /// <summary>
        /// The number of replicas that must return before a delete is considered a success.
        /// </summary>
        /// <value>The RW Value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public Either<uint, string> RwVal { get; private set; }

        /// <summary>
        /// The number of replicas that must commit to durable storage and respond before a write is considered a success. 
        /// </summary>
        /// <value>The DW value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public Either<uint, string> DwVal { get; private set; }

        /// <summary>
        /// The number of replicas that must respond before a write is considered a success.
        /// </summary>
        /// <value>The W value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public Either<uint, string> WVal { get; private set; }

        /// <summary>
        /// The number of primary replicas that must respond before a read is considered a success.
        /// </summary>
        /// <value>The PR value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public Either<uint, string> PrVal { get; private set; }

        /// <summary>
        /// The number of primary replicas that must respond before a write is considered a success.
        /// </summary>
        /// <value>The PW value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public Either<uint, string> PwVal { get; private set; }

        /// <summary>
        /// An indicator of whether search indexing is enabled on the bucket.
        /// </summary>
        public bool SearchEnabled
        {
            get { return PreCommitHooks != null && PreCommitHooks.FirstOrDefault(x => Equals(x, RiakErlangCommitHook.RiakSearchCommitHook)) != null; }
            set { SetSearch(value); }
        }

        public RiakBucketProperties SetBasicQuorum(bool value)
        {
            BasicQuorum = value;
            return this;
        }

        public RiakBucketProperties SetNotFoundOk(bool value)
        {
            NotFoundOk = value;
            return this;
        }

        public RiakBucketProperties SetAllowMultiple(bool value)
        {
            AllowMultiple = value;
            return this;
        }

        public RiakBucketProperties SetLastWriteWins(bool value)
        {
            LastWriteWins = value;
            return this;
        }

        public RiakBucketProperties SetNVal(uint value)
        {
            NVal = value;
            return this;
        }

        public RiakBucketProperties SetRVal(string value)
        {
            return WriteQuorum(value, v => RVal = v);
        }

        public RiakBucketProperties SetRVal(uint value)
        {
            return WriteQuorum(value, v => RVal = v);
        }

        public RiakBucketProperties SetRwVal(string value)
        {
            return WriteQuorum(value, v => RwVal = v);
        }

        public RiakBucketProperties SetRwVal(uint value)
        {
            return WriteQuorum(value, v => RwVal = v);
        }

        public RiakBucketProperties SetDwVal(string value)
        {
            return WriteQuorum(value, v => DwVal = v);
        }

        public RiakBucketProperties SetDwVal(uint value)
        {
            return WriteQuorum(value, v => DwVal = v);
        }

        public RiakBucketProperties SetWVal(string value)
        {
            return WriteQuorum(value, v => WVal = v);
        }

        public RiakBucketProperties SetWVal(uint value)
        {
            return WriteQuorum(value, v => WVal = v);
        }

        public RiakBucketProperties SetPrVal(string value)
        {
            return WriteQuorum(value, v => PrVal = v);
        }

        public RiakBucketProperties SetPrVal(uint value)
        {
            return WriteQuorum(value, v => PrVal = v);
        }

        public RiakBucketProperties SetPwVal(string value)
        {
            return WriteQuorum(value, var => PwVal = var);
        }

        public RiakBucketProperties SetPwVal(uint value)
        {
            return WriteQuorum(value, var => PwVal = var);
        }

        public RiakBucketProperties SetBackend(string backend)
        {
            Backend = backend;
            return this;
        }

        /// <summary>
        /// Enable or disable search on a bucket.
        /// </summary>
        /// <param name="enable">Set to <i>true</i> to enable search on this bucket, or <i>false</i>
        /// to disable it.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>Enabling search on a bucket in Riak requires the adding of a pre-commit hook.
        /// This helper function abstracts this problem from the user so that they don't have to do it
        /// themselves. When adding or removing any form of pre or post commit hook in Riak via any
        /// client, it is a very good idea to first get the bucket properties from Riak, make changes,
        /// then set the properties back. This prevents accidental removal of other pre or post commit
        /// hooks that might have been added beforehand.</remarks>
        public RiakBucketProperties SetSearch(bool enable)
        {
            if (enable)
            {
                AddPreCommitHook(RiakErlangCommitHook.RiakSearchCommitHook);
            }
            else
            {
                RemovePreCommitHook(RiakErlangCommitHook.RiakSearchCommitHook);
            }

            return this;
        }

        public RiakBucketProperties RemovePreCommitHook(IRiakPreCommitHook commitHook)
        {
            if (PreCommitHooks != null)
            {
                PreCommitHooks.RemoveAll(x => Equals(x, commitHook));
            }

            return this;
        }

        public RiakBucketProperties RemovePostCommitHook(IRiakPostCommitHook commitHook)
        {
            if (PostCommitHooks != null)
            {
                PostCommitHooks.RemoveAll(x => Equals(x, commitHook));
            }

            return this;
        }

        public RiakBucketProperties AddPreCommitHook(IRiakPreCommitHook commitHook)
        {
            var hooks = PreCommitHooks ?? (PreCommitHooks = new List<IRiakPreCommitHook>());

            if (!hooks.Any(x => Equals(x, commitHook)))
            {
                hooks.Add(commitHook);
            }

            return this;
        }

        public RiakBucketProperties AddPostCommitHook(IRiakPostCommitHook commitHook)
        {
            var hooks = PostCommitHooks ?? (PostCommitHooks = new List<IRiakPostCommitHook>());
            
            if (!hooks.Any(x => Equals(x, commitHook)))
            {
                hooks.Add(commitHook);
            }

            return this;
        }

        public RiakBucketProperties ClearPreCommitHooks()
        {
            (PreCommitHooks ?? (PreCommitHooks = new List<IRiakPreCommitHook>())).Clear();
            return this;
        }

        public RiakBucketProperties ClearPostCommitHooks()
        {
            (PostCommitHooks ?? (PostCommitHooks = new List<IRiakPostCommitHook>())).Clear();
            return this;
        }

        private RiakBucketProperties WriteQuorum(string value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(new Either<uint, string>(value));
            return this;
        }

        private RiakBucketProperties WriteQuorum(uint value, Action<Either<uint, string>> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(new Either<uint, string>(value));
            return this;
        }

        public RiakBucketProperties()
        {
        }

        public RiakBucketProperties(RiakRestResponse response)
        {
            System.Diagnostics.Debug.Assert(response.ContentType == RiakConstants.ContentTypes.ApplicationJson);

            var json = JObject.Parse(response.Body);
            var props = (JObject)json["props"];
            NVal = props.Value<uint?>("n_val");
            AllowMultiple = props.Value<bool?>("allow_mult");
            LastWriteWins = props.Value<bool?>("last_write_wins");
            Backend = props.Value<string>("backend");
            NotFoundOk = props.Value<bool?>("notfound_ok");
            BasicQuorum = props.Value<bool?>("basic_quorum");

            ReadQuorum(props, "r", v => RVal = v);
            ReadQuorum(props, "rw", v => RwVal = v);
            ReadQuorum(props, "dw", v => DwVal = v);
            ReadQuorum(props, "w", v => WVal = v);
            ReadQuorum(props, "pr", v => PrVal = v);
            ReadQuorum(props, "pw", v => PwVal = v);

            var preCommitHooks = props.Value<JArray>("precommit");
            if(preCommitHooks.Count > 0)
            {
                PreCommitHooks = preCommitHooks.Cast<JObject>().Select(LoadPreCommitHook).ToList();
            }

            var postCommitHooks = props.Value<JArray>("postcommit");
            if(postCommitHooks.Count > 0)
            {
                PostCommitHooks = postCommitHooks.Cast<JObject>().Select(LoadPostCommitHook).ToList();
            }
        }

        private static IRiakPreCommitHook LoadPreCommitHook(JObject hook)
        {
            JToken token;
            if(hook.TryGetValue("name", out token))
            {
                // must be a javascript hook
                return new RiakJavascriptCommitHook(token.Value<string>());
            }

            // otherwise it has to be erlang
            return new RiakErlangCommitHook(hook.Value<string>("mod"), hook.Value<string>("fun"));
        }

        private static IRiakPostCommitHook LoadPostCommitHook(JObject hook)
        {
            // only erlang hooks are supported
            return new RiakErlangCommitHook(hook.Value<string>("mod"), hook.Value<string>("fun"));
        }

        private static void ReadQuorum(JObject props, string key, Action<Either<uint, string>> setter)
        {
            if (props[key] == null) return;

            if(props[key].Type == JTokenType.String)
            {
                setter(new Either<uint, string>(props.Value<string>(key)));
            }
            else
            {
                setter(new Either<uint, string>(props.Value<uint>(key)));
            }
        }

        internal RiakBucketProperties(RpbBucketProps bucketProps)
        : this()
        {
            AllowMultiple = bucketProps.allow_mult;
            NVal = bucketProps.n_val;
        }

        internal RpbBucketProps ToMessage()
        {
            var message = new RpbBucketProps();
            if(AllowMultiple.HasValue)
            {
                message.allow_mult = AllowMultiple.Value;
            }
            if(NVal.HasValue)
            {
                message.n_val = NVal.Value;
            }
            return message;
        }

        internal string ToJsonString()
        {
            var sb = new StringBuilder();

            using(var sw = new StringWriter(sb))
            using(JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartObject();
                jw.WritePropertyName("props");
                jw.WriteStartObject();
                jw.WriteNullableProperty("n_val", NVal)
                .WriteNullableProperty("allow_mult", AllowMultiple)
                .WriteNullableProperty("last_write_wins", LastWriteWins)
                .WriteEither("r", RVal)
                .WriteEither("rw", RwVal)
                .WriteEither("dw", DwVal)
                .WriteEither("w", WVal)
                .WriteEither("pr", PrVal)
                .WriteEither("pw", PwVal)
                .WriteNonNullProperty("backend", Backend)
                .WriteNullableProperty("notfound_ok", NotFoundOk)
                .WriteNullableProperty("basic_quorum", BasicQuorum);

                if(PreCommitHooks != null)
                {
                    jw.WritePropertyName("precommit");
                    jw.WriteStartArray();
                    PreCommitHooks.ForEach(hook => hook.WriteJson(jw));
                    jw.WriteEndArray();
                }

                if(PostCommitHooks != null)
                {
                    jw.WritePropertyName("postcommit");
                    jw.WriteStartArray();
                    PostCommitHooks.ForEach(hook => hook.WriteJson(jw));
                    jw.WriteEndArray();
                }

                jw.WriteEndObject();
                jw.WriteEndObject();
            }

            return sb.ToString();
        }
    }
}

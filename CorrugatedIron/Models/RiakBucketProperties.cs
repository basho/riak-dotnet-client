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
        private bool? _addHooks;

        public bool? LastWriteWins { get; private set; }
        public uint? NVal { get; private set; }
        public bool? AllowMultiple { get; private set; }
        public string Backend { get; private set; }
        public List<IRiakPreCommitHook> PreCommitHooks { get; private set; }
        public List<IRiakPostCommitHook> PostCommitHooks { get; private set; }
        public bool? NotFoundOk { get; private set; }
        public bool? BasicQuorum { get; private set; }
        /// <summary>
        /// If the length of the vector clock is larger than BigVclock, vector clocks will be pruned.
        /// </summary>
        /// <remarks>See http://docs.basho.com/riak/latest/theory/concepts/Vector-Clocks/#Vector-Clock-Pruning </remarks>
        public uint? BigVclock { get; private set; }
        /// <summary>
        /// If the length of the vector clock is smaller than SmallVclock, vector clocks will not be pruned.
        /// </summary>
        /// <remarks>See http://docs.basho.com/riak/latest/theory/concepts/Vector-Clocks/#Vector-Clock-Pruning </remarks>
        public uint? SmallVclock { get; private set; }
        
        public bool? HasPrecommit { get; private set; }
        public bool? HasPostcommit { get; private set; }

        public bool? Search { get; private set; }

        /// <summary>
        /// The number of replicas that must return before a read is considered a succes.
        /// </summary>
        /// <value>
        /// The R value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        public uint? RVal { get; private set; }

        /// <summary>
        /// The number of replicas that must return before a delete is considered a success.
        /// </summary>
        /// <value>The RW Value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public uint? RwVal { get; private set; }

        /// <summary>
        /// The number of replicas that must commit to durable storage and respond before a write is considered a success. 
        /// </summary>
        /// <value>The DW value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public uint? DwVal { get; private set; }

        /// <summary>
        /// The number of replicas that must respond before a write is considered a success.
        /// </summary>
        /// <value>The W value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public uint? WVal { get; private set; }

        /// <summary>
        /// The number of primary replicas that must respond before a read is considered a success.
        /// </summary>
        /// <value>The PR value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public uint? PrVal { get; private set; }

        /// <summary>
        /// The number of primary replicas that must respond before a write is considered a success.
        /// </summary>
        /// <value>The PW value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        public uint? PwVal { get; private set; }

        public RiakConstants.RiakEnterprise.ReplicationMode? ReplicationMode { get; private set; }

        /// <summary>
        /// An indicator of whether search indexing is enabled on the bucket.
        /// </summary>
        public bool SearchEnabled
        {
            get { return (Search.HasValue && Search.Value) || 
                         (PreCommitHooks != null && PreCommitHooks.FirstOrDefault(x => Equals(x, RiakErlangCommitHook.RiakSearchCommitHook)) != null); }
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

        /// <summary>
        /// Enable or disable search on a bucket.
        /// </summary>
        /// <param name="value">Set to <i>true</i> to enable search on this bucket, or <i>false</i>
        /// to disable it.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakBucketProperties SetSearch(bool enable, bool addHooks = false)
        {
            if (addHooks) 
            {
                _addHooks = enable;

                if (enable)
                {
                    AddPreCommitHook(RiakErlangCommitHook.RiakSearchCommitHook);
                }
                else
                {
                    RemovePreCommitHook(RiakErlangCommitHook.RiakSearchCommitHook);
                }                  
            } 
            else 
            {
                Search = enable;
            }
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

        public RiakBucketProperties SetReplicationMode(RiakConstants.RiakEnterprise.ReplicationMode replicationMode)
        {
            ReplicationMode = replicationMode;
            return this;
        }

        public RiakBucketProperties SetBigVclock(uint? bigVclock)
        {
            BigVclock = bigVclock;
            return this;
        }

        public RiakBucketProperties SetSmallVclock(uint? smallVclock)
        {
            SmallVclock = smallVclock;
            return this;
        }

        public RiakBucketProperties RemovePreCommitHook(IRiakPreCommitHook commitHook)
        {
            if (PreCommitHooks != null)
            {
                PreCommitHooks.RemoveAll(x => Equals(x, commitHook));

                if (PreCommitHooks.Count == 0)
                {
                    HasPrecommit = false;
                }
            }

            return this;
        }

        public RiakBucketProperties RemovePostCommitHook(IRiakPostCommitHook commitHook)
        {
            if (PostCommitHooks != null)
            {
                PostCommitHooks.RemoveAll(x => Equals(x, commitHook));

                if (PostCommitHooks.Count == 0)
                {
                    HasPostcommit = false;
                }
            }

            return this;
        }

        public RiakBucketProperties AddPreCommitHook(IRiakPreCommitHook commitHook, bool commitFlags = true)
        {
            var hooks = PreCommitHooks ?? (PreCommitHooks = new List<IRiakPreCommitHook>());

            if (commitHook != null && (commitHook as RiakErlangCommitHook) == RiakErlangCommitHook.RiakSearchCommitHook)
            {
                return SetSearch(true);
            }

            if (!hooks.Any(x => Equals(x, commitHook)))
            {
                hooks.Add(commitHook);
            }

            if (commitFlags)
            {
                HasPrecommit = true;
            }

            return this;
        }

        public RiakBucketProperties AddPostCommitHook(IRiakPostCommitHook commitHook, bool commitFlags = true)
        {
            var hooks = PostCommitHooks ?? (PostCommitHooks = new List<IRiakPostCommitHook>());
            
            if (!hooks.Any(x => Equals(x, commitHook)))
            {
                hooks.Add(commitHook);
            }

            if (commitFlags)
            {
                HasPostcommit = true;
            }

            return this;
        }

        public RiakBucketProperties ClearPreCommitHooks(bool commitFlags = true)
        {
            (PreCommitHooks ?? (PreCommitHooks = new List<IRiakPreCommitHook>())).Clear();

            return this;
        }

        public RiakBucketProperties ClearPostCommitHooks(bool commitFlags = true)
        {
            (PostCommitHooks ?? (PostCommitHooks = new List<IRiakPostCommitHook>())).Clear();

            return this;
        }

        private RiakBucketProperties WriteQuorum(string value, Action<uint> setter)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one", "default" }.Contains(value), "Incorrect quorum value");

            setter(RiakConstants.QuorumOptionsLookup[value]);
            return this;
        }

        private RiakBucketProperties WriteQuorum(uint value, Action<uint> setter)
        {
            System.Diagnostics.Debug.Assert(value >= 1);

            setter(value);
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

            ReplicationMode = (RiakConstants.RiakEnterprise.ReplicationMode)props.Value<int?>("repl");
        }

        internal RiakBucketProperties(RpbBucketProps bucketProps)
            : this()
        {
            NVal = bucketProps.n_val;
            AllowMultiple = bucketProps.allow_mult;
            LastWriteWins = bucketProps.last_write_wins;
            Backend = bucketProps.backend.FromRiakString();
            NotFoundOk = bucketProps.notfound_ok;
            BasicQuorum = bucketProps.basic_quorum;

            HasPrecommit = bucketProps.has_precommit;
            HasPostcommit = bucketProps.has_postcommit;

            RVal = bucketProps.r;
            RwVal = bucketProps.rw;
            DwVal = bucketProps.dw;
            WVal = bucketProps.w;
            PrVal = bucketProps.pr;
            PwVal = bucketProps.pw;

            Search = bucketProps.search;

            HasPrecommit = bucketProps.has_precommit;
            HasPostcommit = bucketProps.has_postcommit;

            var preCommitHooks = bucketProps.precommit;
            if (preCommitHooks.Count > 0)
            {
                PreCommitHooks = preCommitHooks.Select(LoadPreCommitHook).ToList();
            }

            var postCommitHooks = bucketProps.postcommit;
            if (postCommitHooks.Count > 0)
            {
                PostCommitHooks = postCommitHooks.Select(LoadPostCommitHook).ToList();
            }

            ReplicationMode = (RiakConstants.RiakEnterprise.ReplicationMode)bucketProps.repl;
        }

        private static IRiakPreCommitHook LoadPreCommitHook(RpbCommitHook hook)
        {
            if (hook.modfun == null)
            {
                return new RiakJavascriptCommitHook(hook.name.FromRiakString());
            }

            return new RiakErlangCommitHook(hook.modfun.module.FromRiakString(),
                                            hook.modfun.function.FromRiakString());
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

        private static IRiakPostCommitHook LoadPostCommitHook(RpbCommitHook hook)
        {
            return new RiakErlangCommitHook(hook.modfun.module.FromRiakString(),
                                            hook.modfun.function.FromRiakString());
        }

        private static void ReadQuorum(JObject props, string key, Action<uint> setter)
        {
            if (props[key] == null) return;


            setter(props[key].Type == JTokenType.String
                       ? RiakConstants.QuorumOptionsLookup[props.Value<string>(key)]
                       : props.Value<uint>(key));
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

            if (LastWriteWins.HasValue)
            {
                message.last_write_wins = LastWriteWins.Value;
            }

            if (RVal.HasValue)
            {
                message.r = RVal.Value;
            }

            if (RwVal.HasValue)
            {
                message.rw = RwVal.Value;
            }

            if (DwVal.HasValue)
            {
                message.dw = DwVal.Value;
            }

            if (WVal.HasValue)
            {
                message.w = WVal.Value;
            }

            if (PrVal.HasValue)
            {
                message.pr = PrVal.Value;
            }

            if (PwVal.HasValue)
            {
                message.pw = PwVal.Value;
            }

            if (Search.HasValue)
            {
                message.search = Search.Value;
            }

            if (HasPrecommit.HasValue)
            {
                message.has_precommit = HasPrecommit.Value;
            }

            // Due to the amusing differences between 1.3 and 1.4 we've added
            // this elegant shim to figure out if we should add commit hooks,
            // remove them, or just wander around setting the Search boolean.
            if (_addHooks.HasValue)
            {
                if (_addHooks.Value)
                {
                    AddPreCommitHook(RiakErlangCommitHook.RiakSearchCommitHook);
                }
                else
                {
                    RemovePreCommitHook(RiakErlangCommitHook.RiakSearchCommitHook);
                }
            }

            if (PreCommitHooks != null)
            {
                PreCommitHooks.ForEach(h =>
                    {
                        var hook = h.ToRpbCommitHook();
                        if (!message.precommit.Any(x => Equals(x, hook)))
                            message.precommit.Add(hook);
                    });
            }

            if (HasPostcommit.HasValue)
            {
                message.has_postcommit = HasPostcommit.Value;
            }

            if (PostCommitHooks != null)
            {
                PostCommitHooks.ForEach(h =>
                    {
                        var hook = h.ToRpbCommitHook();
                        if (!message.postcommit.Any(x => Equals(x, hook)))
                            message.postcommit.Add(hook);
                    });
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
                  .WriteNullableProperty("r", RVal)
                  .WriteNullableProperty("rw", RwVal)
                  .WriteNullableProperty("dw", DwVal)
                  .WriteNullableProperty("w", WVal)
                  .WriteNullableProperty("pr", PrVal)
                  .WriteNullableProperty("pw", PwVal)
                  .WriteNonNullProperty("backend", Backend)
                  .WriteNullableProperty("notfound_ok", NotFoundOk)
                  .WriteNullableProperty("basic_quorum", BasicQuorum)
                  .WriteNullableProperty("has_precommit", HasPrecommit)
                  .WriteNullableProperty("has_postcommit", HasPostcommit);

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

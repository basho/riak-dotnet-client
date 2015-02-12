// <copyright file="RiakBucketProperties.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Extensions;
    using Messages;
    using Models.CommitHook;
    using Models.Rest;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [ComVisible(false)]
    public class RiakBucketProperties : RiakOptions<RiakBucketProperties>
    {
        private bool? addHooks;

        public RiakBucketProperties()
        {
        }

        public RiakBucketProperties(RiakRestResponse response)
        {
            if (response.ContentType != RiakConstants.ContentTypes.ApplicationJson)
            {
                throw new ArgumentOutOfRangeException("response.ContentType must be application/json");
            }

            var json = JObject.Parse(response.Body);
            var props = (JObject)json["props"];

            if (props.Value<uint?>("n_val").HasValue)
            {
                NVal = new NVal(props.Value<uint>("n_val"));
            }

            AllowMultiple = props.Value<bool?>("allow_mult");
            LastWriteWins = props.Value<bool?>("last_write_wins");
            Backend = props.Value<string>("backend");
            NotFoundOk = props.Value<bool?>("notfound_ok");
            BasicQuorum = props.Value<bool?>("basic_quorum");
            Consistent = props.Value<bool?>("consistent");

            ReadQuorum(props, "r", v => this.SetR(new Quorum(v)));
            ReadQuorum(props, "rw", v => this.SetRw(new Quorum(v)));
            ReadQuorum(props, "dw", v => this.SetDw(new Quorum(v)));
            ReadQuorum(props, "w", v => this.SetW(new Quorum(v)));
            ReadQuorum(props, "pr", v => this.SetPr(new Quorum(v)));
            ReadQuorum(props, "pw", v => this.SetPw(new Quorum(v)));

            var preCommitHooks = props.Value<JArray>("precommit");
            if (preCommitHooks.Count > 0)
            {
                PreCommitHooks = preCommitHooks.Cast<JObject>().Select(LoadPreCommitHook).ToList();
            }

            var postCommitHooks = props.Value<JArray>("postcommit");
            if (postCommitHooks.Count > 0)
            {
                PostCommitHooks = postCommitHooks.Cast<JObject>().Select(LoadPostCommitHook).ToList();
            }

            var value = props.Value<int?>("repl");
            if (value != null)
            {
                ReplicationMode = (RiakConstants.RiakEnterprise.ReplicationMode)value;
            }
        }

        internal RiakBucketProperties(RpbBucketProps bucketProps)
        {
            NVal = new NVal(bucketProps.n_val);
            AllowMultiple = bucketProps.allow_mult;
            LastWriteWins = bucketProps.last_write_wins;
            Backend = bucketProps.backend.FromRiakString();
            NotFoundOk = bucketProps.notfound_ok;
            BasicQuorum = bucketProps.basic_quorum;

            HasPrecommit = bucketProps.has_precommit;
            HasPostcommit = bucketProps.has_postcommit;

            R = (Quorum)bucketProps.r;
            Rw = (Quorum)bucketProps.rw;
            Dw = (Quorum)bucketProps.dw;
            W = (Quorum)bucketProps.w;
            Pr = (Quorum)bucketProps.pr;
            Pw = (Quorum)bucketProps.pw;

            LegacySearch = bucketProps.search;

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

            SearchIndex = bucketProps.search_index.FromRiakString();
            DataType = bucketProps.datatype.FromRiakString();

            Consistent = bucketProps.consistent;
        }

        public bool? LastWriteWins { get; private set; }

        public NVal NVal { get; private set; }

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
        public int? BigVclock { get; private set; }

        /// <summary>
        /// If the length of the vector clock is smaller than SmallVclock, vector clocks will not be pruned.
        /// </summary>
        /// <remarks>See http://docs.basho.com/riak/latest/theory/concepts/Vector-Clocks/#Vector-Clock-Pruning </remarks>
        public int? SmallVclock { get; private set; }

        public bool? HasPrecommit { get; private set; }

        public bool? HasPostcommit { get; private set; }

        [Obsolete("Search is deprecated, please use LegacySearch instead.", true)]
        public bool? Search
        {
            get { return LegacySearch; }
            private set { LegacySearch = value; }
        }

        public bool? LegacySearch
        {
            get;
            private set;
        }

        public bool? Consistent { get; private set; }

        public RiakConstants.RiakEnterprise.ReplicationMode ReplicationMode { get; private set; }

        public string SearchIndex { get; private set; }

        [Obsolete("SearchEnabled is deprecated, please use LegacySearchEnabled instead.", true)]
        public bool SearchEnabled
        {
            get { return LegacySearchEnabled; }
        }

        /// <summary>
        /// An indicator of whether legacy search indexing is enabled on the bucket.
        /// </summary>
        public bool LegacySearchEnabled
        {
            get
            {
                return (LegacySearch.HasValue && LegacySearch.Value) ||
                       (PreCommitHooks != null && PreCommitHooks.FirstOrDefault(x => Equals(x, RiakErlangCommitHook.RiakLegacySearchCommitHook)) != null);
            }
        }

        /// <summary>
        /// The DataType (if any) associated with this bucket.
        /// </summary>
        /// <value>A string representation of the DataType assigned to this bucket. Possible values include 'set', 'map', 'counter', or null for no data type </value>
        public string DataType { get; private set; }

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

        [Obsolete("SetSearch is deprecated, please use SetLegacySearch instead.", true)]
        public RiakBucketProperties SetSearch(bool enable, bool addHooks = false)
        {
            return SetLegacySearch(enable, addHooks);
        }

        /// <summary>
        /// Enable or disable legacy search on a bucket.
        /// </summary>
        /// <param name="enable">Set to <i>true</i> to enable legacy search on this bucket, or <i>false</i>
        /// to disable it.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <param name="addHooks">Set to <i>true</i> to add pre commit hook</param>
        public RiakBucketProperties SetLegacySearch(bool enable, bool addHooks = false)
        {
            if (addHooks)
            {
                this.addHooks = enable;

                if (enable)
                {
                    AddPreCommitHook(RiakErlangCommitHook.RiakLegacySearchCommitHook);
                }
                else
                {
                    RemovePreCommitHook(RiakErlangCommitHook.RiakLegacySearchCommitHook);
                }
            }
            else
            {
                LegacySearch = enable;
            }

            return this;
        }

        public RiakBucketProperties SetNVal(NVal value)
        {
            NVal = value;
            return this;
        }

        public RiakBucketProperties SetBackend(string backend)
        {
            Backend = backend;
            return this;
        }

        public RiakBucketProperties SetBigVclock(int? bigVclock)
        {
            BigVclock = bigVclock;
            return this;
        }

        public RiakBucketProperties SetSmallVclock(int? smallVclock)
        {
            SmallVclock = smallVclock;
            return this;
        }

        public RiakBucketProperties SetSearchIndex(string searchIndex)
        {
            SearchIndex = searchIndex;
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

            if (commitHook != null && (commitHook as RiakErlangCommitHook) == RiakErlangCommitHook.RiakLegacySearchCommitHook)
            {
                return SetLegacySearch(true);
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

        internal RpbBucketProps ToMessage()
        {
            var message = new RpbBucketProps();

            if (AllowMultiple.HasValue)
            {
                message.allow_mult = AllowMultiple.Value;
            }

            if (NVal != null)
            {
                message.n_val = NVal;
            }

            if (LastWriteWins.HasValue)
            {
                message.last_write_wins = LastWriteWins.Value;
            }

            if (R != null)
            {
                message.r = R;
            }

            if (Rw != null)
            {
                message.rw = Rw;
            }

            if (Dw != null)
            {
                message.dw = Dw;
            }

            if (W != null)
            {
                message.w = W;
            }

            if (Pr != null)
            {
                message.pr = Pr;
            }

            if (Pw != null)
            {
                message.pw = Pw;
            }

            if (LegacySearch.HasValue)
            {
                message.search = LegacySearch.Value;
            }

            if (HasPrecommit.HasValue)
            {
                message.has_precommit = HasPrecommit.Value;
            }

            // Due to the amusing differences between 1.3 and 1.4 we've added
            // this elegant shim to figure out if we should add commit hooks,
            // remove them, or just wander around setting the Search boolean.
            if (addHooks.HasValue)
            {
                if (addHooks.Value)
                {
                    AddPreCommitHook(RiakErlangCommitHook.RiakLegacySearchCommitHook);
                }
                else
                {
                    RemovePreCommitHook(RiakErlangCommitHook.RiakLegacySearchCommitHook);
                }
            }

            if (PreCommitHooks != null)
            {
                PreCommitHooks.ForEach(h =>
                    {
                        var hook = h.ToRpbCommitHook();
                        if (!message.precommit.Any(x => Equals(x, hook)))
                        {
                            message.precommit.Add(hook);
                        }
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
                        {
                            message.postcommit.Add(hook);
                        }
                    });
            }

            if (!string.IsNullOrEmpty(SearchIndex))
            {
                message.search_index = SearchIndex.ToRiakString();
            }

            return message;
        }

        internal string ToJsonString()
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartObject();
                jw.WritePropertyName("props");
                jw.WriteStartObject();
                jw.WriteNonNullProperty("n_val", NVal)
                  .WriteNullableProperty("allow_mult", AllowMultiple)
                  .WriteNullableProperty("last_write_wins", LastWriteWins)
                  .WriteNonNullProperty("r", R)
                  .WriteNonNullProperty("rw", Rw)
                  .WriteNonNullProperty("dw", Dw)
                  .WriteNonNullProperty("w", W)
                  .WriteNonNullProperty("pr", Pr)
                  .WriteNonNullProperty("pw", Pw)
                  .WriteNonNullProperty("backend", Backend)
                  .WriteNullableProperty("notfound_ok", NotFoundOk)
                  .WriteNullableProperty("basic_quorum", BasicQuorum)
                  .WriteNullableProperty("has_precommit", HasPrecommit)
                  .WriteNullableProperty("has_postcommit", HasPostcommit);

                if (PreCommitHooks != null)
                {
                    jw.WritePropertyName("precommit");
                    jw.WriteStartArray();
                    PreCommitHooks.ForEach(hook => hook.WriteJson(jw));
                    jw.WriteEndArray();
                }

                if (PostCommitHooks != null)
                {
                    jw.WritePropertyName("postcommit");
                    jw.WriteStartArray();
                    PostCommitHooks.ForEach(hook => hook.WriteJson(jw));
                    jw.WriteEndArray();
                }

                if (!string.IsNullOrEmpty(SearchIndex))
                {
                    jw.WriteNonNullProperty("search_index", SearchIndex);
                }

                jw.WriteEndObject();
                jw.WriteEndObject();
            }

            return sb.ToString();
        }

        private static IRiakPreCommitHook LoadPreCommitHook(RpbCommitHook hook)
        {
            if (hook.modfun == null)
            {
                return new RiakJavascriptCommitHook(hook.name.FromRiakString());
            }

            return new RiakErlangCommitHook(
                hook.modfun.module.FromRiakString(),
                hook.modfun.function.FromRiakString());
        }

        private static IRiakPreCommitHook LoadPreCommitHook(JObject hook)
        {
            JToken token;
            if (hook.TryGetValue("name", out token))
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
            return new RiakErlangCommitHook(
                hook.modfun.module.FromRiakString(),
                hook.modfun.function.FromRiakString());
        }

        private static void ReadQuorum(JObject props, string key, Action<uint> setter)
        {
            if (props[key] == null)
            {
                return;
            }

            uint quorumValue = props[key].Type == JTokenType.String ? uint.Parse(props.Value<string>(key)) : props.Value<uint>(key);

            setter(quorumValue);
        }
    }
}
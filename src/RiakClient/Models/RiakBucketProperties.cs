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
    using System.Linq;
    using System.Runtime.InteropServices;
    using CommitHook;
    using Extensions;
    using Messages;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the collection of properties for a Riak bucket.
    /// </summary>
    /// <remarks>
    /// When creating new objects, any properties not set will default to the server default values upon saving.
    /// </remarks>
    [ComVisible(false)]
    public class RiakBucketProperties : RiakOptions<RiakBucketProperties>
    {
        private bool? addHooks;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBucketProperties" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor will create a new <see cref="RiakBucketProperties"/> instance with no properties set.
        /// </remarks>
        public RiakBucketProperties()
        {
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

        /// <summary>
        /// The option to ignore object history (vector clock) when updating an object.
        /// </summary>
        /// <remarks>
        /// Cannot be set to true while <see cref="AllowMultiple"/> is also set to true.
        /// Riak will default this property to <b>false</b>.
        /// See http://docs.basho.com/riak/latest/dev/using/conflict-resolution/ for more information.
        /// </remarks>
        public bool? LastWriteWins { get; private set; }

        /// <summary>
        /// The number of replicas to create when storing data.
        /// </summary>
        /// <remarks>
        /// Riak will default this property to an <see cref="NVal"/> equivalent of <b>3</b>.
        /// See http://docs.basho.com/riak/latest/theory/concepts/Replication/ for more information.
        /// </remarks>
        public NVal NVal { get; private set; }

        /// <summary>
        /// The option to allow sibling objects to be created (concurrent updates) when updating an object.
        /// </summary>
        /// <remarks>
        /// Cannot be set to true while <see cref="LastWriteWins"/> is also set to true.
        /// Riak will default this property to <b>true</b> for buckets within a 
        /// bucket type (other than the default one) in <b>Riak 2.0+</b>.
        /// Riak will default this property to <b>false</b> for buckets within <b>the default bucket type</b> in <b>Riak 2.0+</b>.
        /// Riak will default this property to <b>false</b> for all buckets in <b>Riak 1.0</b>.
        /// See http://docs.basho.com/riak/latest/dev/using/conflict-resolution/ for more information.
        /// </remarks>
        public bool? AllowMultiple { get; private set; }

        /// <summary>
        /// The named backend being used for this bucket when using riak_kv_multi_backend.
        /// Can be the named backend <i>to use</i> when creating bucket properties for new buckets,
        /// or the named backend <i>in use</i> for existing buckets &amp; bucket properties.
        /// </summary>
        /// <remarks>
        /// See http://docs.basho.com/riak/latest/ops/advanced/backends/multi/ for more information.
        /// </remarks>
        public string Backend { get; private set; }
        
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="IRiakPreCommitHook"/> pre-commit hooks.
        /// </summary>
        /// <remarks>
        /// See http://docs.basho.com/riak/latest/dev/using/commit-hooks/ for more information.
        /// </remarks>
        public List<IRiakPreCommitHook> PreCommitHooks { get; private set; }

        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="IRiakPreCommitHook"/> post-commit hooks.
        /// </summary>
        /// <remarks>
        /// See http://docs.basho.com/riak/latest/dev/using/commit-hooks/ for more information.
        /// </remarks>
        public List<IRiakPostCommitHook> PostCommitHooks { get; private set; }

        /// <summary>
        /// When set to true, an object not being found on a Riak node will count towards the R count. 
        ///  </summary>
        /// <remarks>
        /// Riak will default this property to <b>true</b>.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#The-Implications-of-code-notfound_ok-code-
        /// for more information.
        /// </remarks>
        public bool? NotFoundOk { get; private set; }

        /// <summary>
        /// When set to true, Riak will return early in some failure cases.
        /// (eg. when r=1 and you get 2 errors and a success basic_quorum=true would return an error).
        /// Can be used in conjunction when <see cref="NotFoundOk"/>=<b>false</b> to speed up the case an object 
        /// does not exist, thereby only reading a "quorum" of not-founds instead of "N" not-founds.
        /// </summary>
        /// <remarks>
        /// Riak will default this property to <b>false</b>.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#Early-Failure-Return-with-code-basic_quorum-code-
        /// for more information.
        /// </remarks>
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

        /// <summary>
        /// Indicates whether the bucket properties have any pre-commit hooks.
        /// </summary>
        public bool? HasPrecommit { get; private set; }

        /// <summary>
        /// Indicates whether the bucket properties have any post-commit hooks.
        /// </summary>
        public bool? HasPostcommit { get; private set; }

        /// <summary>
        /// Indicates whether Legacy (Riak 1.0) Search is enabled for this bucket.
        /// </summary>
        [Obsolete("Search is deprecated, please use LegacySearch instead.", true)]
        public bool? Search
        {
            get { return LegacySearch; }
            private set { LegacySearch = value; }
        }

        /// <summary>
        /// Indicates whether Legacy (Riak 1.0) Search is enabled for this bucket.
        /// </summary>
        public bool? LegacySearch
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether strong consistency is enabled on this bucket.
        /// </summary>
        public bool? Consistent { get; private set; }

        /// <summary>
        /// Indicates which Riak Enterprise Edition replication modes are active for this bucket.
        /// </summary>
        public RiakConstants.RiakEnterprise.ReplicationMode ReplicationMode { get; private set; }

        /// <summary>
        /// Indicates whether Riak Search (2.0+) is enabled for this bucket, and which Search Index is assigned to it.
        /// </summary>
        public string SearchIndex { get; private set; }

        /// <summary>
        /// An indicator of whether legacy search indexing is enabled on the bucket. Please use <see cref="LegacySearchEnabled"/> instead.
        /// </summary>
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
        /// <remarks>
        /// A string representation of the DataType assigned to this bucket. 
        /// Possible values include 'set', 'map', 'counter', or null for no data type.
        /// </remarks>
        public string DataType { get; private set; }

        /// <summary>
        /// Fluent setter for the <see cref="BasicQuorum"/> property.
        /// When set to true, Riak will return early in some failure cases.
        /// (eg. when r=1 and you get 2 errors and a success basic_quorum=true would return an error).
        /// Can be used in conjunction when <see cref="NotFoundOk"/>=<b>false</b> to speed up the case an object 
        /// does not exist, thereby only reading a "quorum" of not-founds instead of "N" not-founds.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#Early-Failure-Return-with-code-basic_quorum-code-
        /// for more information.
        /// </remarks>
        public RiakBucketProperties SetBasicQuorum(bool value)
        {
            BasicQuorum = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="NotFoundOk"/> property.
        /// Enable or disable notfound_ok on a bucket.
        /// When set to true, an object not being found on a Riak node will count towards the R count. 
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// See http://docs.basho.com/riak/latest/dev/advanced/replication-properties/#The-Implications-of-code-notfound_ok-code-
        /// for more information.
        /// </remarks>
        public RiakBucketProperties SetNotFoundOk(bool value)
        {
            NotFoundOk = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="AllowMultiple"/> property.
        /// Enable or disable allow_mult on a bucket.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakBucketProperties SetAllowMultiple(bool value)
        {
            AllowMultiple = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="LastWriteWins"/> property.
        /// Enable or disable last_write_wins on a bucket.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakBucketProperties SetLastWriteWins(bool value)
        {
            LastWriteWins = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="LegacySearch"/> property.
        /// Enable or disable legacy search on a bucket.
        /// </summary>
        /// <param name="enable">
        /// Set to <i>true</i> to enable legacy search on this bucket, or <i>false</i> to disable it.
        /// </param>
        /// <param name="addHooks">Set to <i>true</i> to add pre commit hook.</param>
        /// <returns>A reference to the current properties object.</returns>
        [Obsolete("SetSearch is deprecated, please use SetLegacySearch instead.", true)]
        public RiakBucketProperties SetSearch(bool enable, bool addHooks = false)
        {
            return SetLegacySearch(enable, addHooks);
        }

        /// <summary>
        /// Fluent setter for the <see cref="LegacySearch"/> property.
        /// Enable or disable legacy search on a bucket.
        /// </summary>
        /// <param name="enable">
        /// Set to <i>true</i> to enable legacy search on this bucket, or <i>false</i> to disable it.
        /// </param>
        /// <returns>A reference to the current properties object.</returns>
        /// <param name="addHooks">Set to <i>true</i> to add pre commit hook.</param>
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

        /// <summary>
        /// Fluent setter for the <see cref="NVal"/> property.
        /// The number of replicas to create when storing data.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakBucketProperties SetNVal(NVal value)
        {
            NVal = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="Backend"/> property.
        /// Sets the named backend being used for this bucket when using riak_kv_multi_backend.
        /// </summary>
        /// <param name="backend">The backend to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakBucketProperties SetBackend(string backend)
        {
            Backend = backend;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="BigVclock"/> property.
        /// If the length of the vector clock is larger than BigVclock, vector clocks will be pruned.
        /// </summary>
        /// <param name="bigVclock">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakBucketProperties SetBigVclock(int? bigVclock)
        {
            BigVclock = bigVclock;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="SmallVclock"/> property.
        /// If the length of the vector clock is smaller than SmallVclock, vector clocks will not be pruned.
        /// </summary>
        /// <param name="smallVclock">The value to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// This property is not typically modified.
        /// </remarks>
        public RiakBucketProperties SetSmallVclock(int? smallVclock)
        {
            SmallVclock = smallVclock;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="SearchIndex"/> property.
        /// Sets the Search Index to use when indexing data for search.
        /// </summary>
        /// <param name="searchIndex">The name of the index to set the property to.</param>
        /// <returns>A reference to the current properties object.</returns>
        public RiakBucketProperties SetSearchIndex(string searchIndex)
        {
            SearchIndex = searchIndex;
            return this;
        }

        /// <summary>
        /// Remove a pre-commit hook from the bucket properties.
        /// </summary>
        /// <param name="commitHook">The <see cref="IRiakPreCommitHook"/> to remove.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// Pre/Post-commit hooks are not typically modified.
        /// </remarks>
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

        /// <summary>
        /// Remove a post-commit hook from the bucket properties.
        /// </summary>
        /// <param name="commitHook">The <see cref="IRiakPreCommitHook"/> to remove.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// Pre/Post-commit hooks are not typically modified.
        /// </remarks>
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

        /// <summary>
        /// Add a pre-commit hook to the bucket properties.
        /// </summary>
        /// <param name="commitHook">The <see cref="IRiakPreCommitHook"/> to add.</param>
        /// <param name="commitFlags">The option to set <see cref="HasPrecommit"/> to true at the same time.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// Pre/Post-commit hooks are not typically modified.
        /// </remarks>
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

        /// <summary>
        /// Add a post-commit hook to the bucket properties.
        /// </summary>
        /// <param name="commitHook">The commit hook to add.</param>
        /// <param name="commitFlags">The option to set <see cref="HasPostcommit"/> to true at the same time.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// Pre/Post-commit hooks are not typically modified.
        /// </remarks>
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

        /// <summary>
        /// Clear any pre-commit hooks from the bucket properties.
        /// </summary>
        /// <param name="commitFlags">Not used.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// Pre/Post-commit hooks are not typically modified.
        /// </remarks>
        public RiakBucketProperties ClearPreCommitHooks(bool commitFlags = true)
        {
            (PreCommitHooks ?? (PreCommitHooks = new List<IRiakPreCommitHook>())).Clear();

            return this;
        }

        /// <summary>
        /// Clear any post-commit hooks from the bucket properties.
        /// </summary>
        /// <param name="commitFlags">Not used.</param>
        /// <returns>A reference to the current properties object.</returns>
        /// <remarks>
        /// Pre/Postcommit hooks are not typically modified.
        /// </remarks>
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
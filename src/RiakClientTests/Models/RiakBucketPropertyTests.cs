// <copyright file="RiakBucketPropertyTests.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Models
{
    using System.Linq;
    using NUnit.Framework;
    using RiakClient.Extensions;
    using RiakClient.Models;
    using RiakClient.Models.CommitHook;

    [TestFixture, UnitTest]
    public class RiakBucketPropertyTests
    {
        [Test]
        public void WhenEnablingRiakLegacySearchOnBucketPreCommitHookIsAdded()
        {
            var props = new RiakBucketProperties()
                .AddPreCommitHook(new RiakErlangCommitHook("foo", "bar"))
                .SetLegacySearch(true);

            props.PreCommitHooks.Count.ShouldEqual(1);
            props.LegacySearch.ShouldEqual(true);
        }

        [Test]
        public void WhenDisablingRiakLegacySearchOnBucketPreCommitHookIsRemoved()
        {
            var props = new RiakBucketProperties()
                .AddPreCommitHook(new RiakErlangCommitHook("foo", "bar"))
                .SetLegacySearch(true);

            props.PreCommitHooks.Count.ShouldEqual(1);
            props.LegacySearch.ShouldEqual(true);

            props.SetLegacySearch(false);
            props.LegacySearch.ShouldNotEqual(true);
            ((RiakErlangCommitHook)props.PreCommitHooks[0]).Function.ShouldEqual("bar");
        }

        [Test]
        public void WhenDisablingRiakLegacySearchOnBucketWithoutLegacySearchEnabledPreCommitHooksAreLeftAlone()
        {
            var props = new RiakBucketProperties()
                .AddPreCommitHook(new RiakErlangCommitHook("foo", "bar"))
                .SetLegacySearch(false);

            props.PreCommitHooks.Count.ShouldEqual(1);
        }

        [Test]
        public void WhenAddingAndRemovingPreAndPostCommitHooksThingsWorkAsExpected()
        {
            var props = new RiakBucketProperties()
                .AddPreCommitHook(new RiakJavascriptCommitHook("some_fun"))
                .AddPreCommitHook(new RiakJavascriptCommitHook("some_fun"))
                .AddPreCommitHook(new RiakErlangCommitHook("mod", "fun"))
                .AddPreCommitHook(new RiakErlangCommitHook("mod", "fun"))
                .AddPreCommitHook(new RiakErlangCommitHook("riak_search_kv_hook", "precommit"))
                .AddPreCommitHook(new RiakErlangCommitHook("riak_search_kv_hook", "precommit"))
                .AddPreCommitHook(RiakErlangCommitHook.RiakLegacySearchCommitHook)
                .AddPostCommitHook(new RiakErlangCommitHook("mod", "fun"))
                .AddPostCommitHook(new RiakErlangCommitHook("mod", "fun"));

            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PostCommitHooks.Count.ShouldEqual(1);

            // setting search should not change commit hook count
            props.SetLegacySearch(true);
            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PostCommitHooks.Count.ShouldEqual(1);

            // setting search should not change commit hook count
            props.SetLegacySearch(false);
            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PostCommitHooks.Count.ShouldEqual(1);

            props.PreCommitHooks.Where(x => x is RiakErlangCommitHook).Cast<RiakErlangCommitHook>()
                .Any(x => x.Function == RiakErlangCommitHook.RiakLegacySearchCommitHook.Function
                    && x.Function == RiakErlangCommitHook.RiakLegacySearchCommitHook.Function)
                .ShouldBeFalse();
        }

        // TODO: perhaps add some tests to make sure that pre and post commit hooks, along with other
        // TODO: properties don't end up in the payload when they aren't explicitly set/added/removed


        [Test]
        public void WhenEnablingSearchOnABucketItSetsPropertiesCorrectly()
        {
            var props = new RiakBucketProperties().SetSearchIndex("foo");
            Assert.AreEqual("foo", props.SearchIndex);

            var rpbMessageProps = props.ToMessage();
            Assert.AreEqual("foo", rpbMessageProps.search_index.FromRiakString());
        }
    }
}

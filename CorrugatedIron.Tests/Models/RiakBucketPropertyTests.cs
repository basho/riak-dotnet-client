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

using System.Linq;
using CorrugatedIron.Models;
using CorrugatedIron.Models.CommitHook;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models
{
    [TestFixture]
    public class RiakBucketPropertyTests
    {
        [Test]
        public void WhenEnablingRiakSearchOnBucketPreCommitHookIsAdded()
        {
            var props = new RiakBucketProperties()
                .AddPreCommitHook(new RiakErlangCommitHook("foo", "bar"))
                .SetSearch(true);

            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PreCommitHooks[1].ShouldEqual(RiakErlangCommitHook.RiakSearchCommitHook);
        }

        [Test]
        public void WhenDisablingRiakSearchOnBucketPreCommitHookIsRemoved()
        {
            var props = new RiakBucketProperties()
                .AddPreCommitHook(new RiakErlangCommitHook("foo", "bar"))
                .SetSearch(true);

            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PreCommitHooks[1].ShouldEqual(RiakErlangCommitHook.RiakSearchCommitHook);

            props.SetSearch(false);
            props.PreCommitHooks.Count.ShouldEqual(1);
            ((RiakErlangCommitHook)props.PreCommitHooks[0]).Function.ShouldEqual("bar");
        }

        [Test]
        public void WhenDisablingRiakSearchOnBucketWithoutSearchEnabledPreCommitHooksAreLeftAlone()
        {
            var props = new RiakBucketProperties()
                .AddPreCommitHook(new RiakErlangCommitHook("foo", "bar"))
                .SetSearch(false);

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
                .AddPreCommitHook(RiakErlangCommitHook.RiakSearchCommitHook)
                .AddPostCommitHook(new RiakErlangCommitHook("mod", "fun"))
                .AddPostCommitHook(new RiakErlangCommitHook("mod", "fun"));

            props.PreCommitHooks.Count.ShouldEqual(3);
            props.PostCommitHooks.Count.ShouldEqual(1);

            props.SetSearch(true);
            props.PreCommitHooks.Count.ShouldEqual(3);
            props.PostCommitHooks.Count.ShouldEqual(1);

            props.SetSearch(false);
            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PostCommitHooks.Count.ShouldEqual(1);

            props.PreCommitHooks.Where(x => x is RiakErlangCommitHook).Cast<RiakErlangCommitHook>()
                .Any(x => x.Function == RiakErlangCommitHook.RiakSearchCommitHook.Function
                    && x.Function == RiakErlangCommitHook.RiakSearchCommitHook.Function)
                .ShouldBeFalse();
        }

        // TODO: perhaps add some tests to make sure that pre and post commit hooks, along with other
        // TODO: properties don't end up in the payload when they aren't explicitly set/added/removed
    }
}
